using Aro.Admin.Application.Services;
using Aro.Admin.Application.Services.DataServices;
using Aro.Admin.Application.Services.DTOs.ServiceResponses;
using Aro.Admin.Domain.Entities;
using Aro.Admin.Domain.Repository;
using Aro.Admin.Domain.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Aro.Admin.Infrastructure.Services;

public partial class AuthenticationService(IHasher haser, IUserService userService, IAccessTokenService accessTokenService, IRefreshTokenService refreshTokenService, IRepositoryManager repositoryManager, IUniqueIdGenerator idGenerator, ITokenBlackListService tokenBlackListService, IActiveAccessTokenService activeAccessTokenService, ErrorCodes errorCodes, ILogManager<AuthenticationService> logger) : IAuthenticationService
{
    private readonly IRefreshTokenRepository refreshTokenRepo = repositoryManager.RefreshTokenRepository;

    public async Task<CompositeToken> Authenticate(string email, string password, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Starting {MethodName}", nameof(Authenticate));
        
        var user = await userService.GetUserByEmail(email, false, true, cancellationToken).ConfigureAwait(false);
        logger.LogDebug("Retrieved user for authentication, userId: {UserId}, email: {Email}", user.Id, email);
        
        var isPasswordCorrect = haser.Verify(password, user.PasswordHash);
        logger.LogDebug("Password verification completed for email: {Email}, result: {IsValid}", email, isPasswordCorrect);

        if (!isPasswordCorrect)
        {
            logger.LogWarn("Authentication failed for email: {Email} - invalid password", email);
            throw new AroException(errorCodes.INVALID_PASSWORD, $"Invalid password for user {email}.");
        }

        logger.LogInfo("Generating access token for user: {UserId}", user.Id);
        var accessToken = await accessTokenService.GenerateAccessToken(user.Id, cancellationToken).ConfigureAwait(false);
        logger.LogDebug("Access token generated for user: {UserId}, expires: {Expiry}", user.Id, accessToken.Expiry);

        logger.LogInfo("Generating refresh token for user: {UserId}", user.Id);
        var refreshToken = await refreshTokenService.GenerateRefreshToken(cancellationToken).ConfigureAwait(false);
        logger.LogDebug("Refresh token generated for user: {UserId}, expires: {Expiry}", user.Id, refreshToken.ExpiresAt);

        var refreshEntity = new Domain.Entities.RefreshToken
        {
            Id = idGenerator.Generate(),
            UserId = user.Id,
            TokenHash = haser.Hash(refreshToken.Token),
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = refreshToken.ExpiresAt,
        };
        logger.LogDebug("Created refresh token entity for user: {UserId}, tokenId: {TokenId}", user.Id, refreshEntity.Id);

        await refreshTokenRepo.Create(refreshEntity, cancellationToken).ConfigureAwait(false);
        await repositoryManager.SaveChanges(cancellationToken).ConfigureAwait(false);
        logger.LogDebug("Refresh token entity saved to database for user: {UserId}", user.Id);

        var response = new CompositeToken
        {
            OldRefreshTokenHash = string.Empty,
            UserId = user.Id,
            RefreshTokenId = refreshEntity.Id,
            AccessToken = accessToken.Token,
            RefreshToken = refreshToken.Token,
            AccessTokenExpiry = accessToken.Expiry,
            RefreshTokenExpiry = refreshToken.ExpiresAt,
            AccessTokenIdentifier = accessToken.TokenIdentifier
        };

        logger.LogInfo("Authentication successful for email: {Email}, userId: {UserId}", email, user.Id);

        logger.LogDebug("Completed {MethodName}", nameof(Authenticate));
        return response;
    }

    public async Task<bool> Logout(Guid userId, string refreshToken, string accessTokenIdentifier, DateTime accessTokenExpiry, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Starting {MethodName}", nameof(Logout));
        
        logger.LogDebug("Revoking refresh token for user: {UserId}", userId);
        var result = await refreshTokenService.Revoke(userId, refreshToken, cancellationToken).ConfigureAwait(false);
        logger.LogDebug("Refresh token revocation result for user: {UserId}, success: {Success}", userId, result);

        logger.LogDebug("Blacklisting access token: {AccessTokenIdentifier}, expiry: {Expiry}", accessTokenIdentifier, accessTokenExpiry);
        await tokenBlackListService.BlackList(accessTokenIdentifier, accessTokenExpiry, cancellationToken).ConfigureAwait(false);
        logger.LogDebug("Access token blacklisted: {AccessTokenIdentifier}", accessTokenIdentifier);

        logger.LogInfo("Logout completed for user: {UserId}, refreshTokenRevoked: {RefreshTokenRevoked}", userId, result);
        
        logger.LogDebug("Completed {MethodName}", nameof(Logout));
        return result;
    }

    public async Task LogoutAll(Guid userId, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Starting {MethodName}", nameof(LogoutAll));
        
        logger.LogDebug("Revoking all refresh tokens for user: {UserId}", userId);
        await refreshTokenService.RevokeAll(userId, cancellationToken).ConfigureAwait(false);
        logger.LogDebug("All refresh tokens revoked for user: {UserId}", userId);

        logger.LogDebug("Retrieving active access tokens for user: {UserId}", userId);
        var activeAccessTokens = await activeAccessTokenService.GetActiveTokens(userId, cancellationToken).ConfigureAwait(false);
        logger.LogDebug("Found {TokenCount} active access tokens for user: {UserId}", activeAccessTokens.Count, userId);

        logger.LogDebug("Blacklisting {TokenCount} active access tokens for user: {UserId}", activeAccessTokens.Count, userId);
        activeAccessTokens.ForEach(async t => await tokenBlackListService.BlackList(t.TokenIdentifier, t.Expiry, cancellationToken).ConfigureAwait(false));
        logger.LogDebug("All active access tokens blacklisted for user: {UserId}", userId);

        logger.LogDebug("Removing all tokens from active token service for user: {UserId}", userId);
        await activeAccessTokenService.RemoveAllTokens(userId, cancellationToken).ConfigureAwait(false);
        logger.LogDebug("All tokens removed from active token service for user: {UserId}", userId);

        logger.LogInfo("Logout all sessions completed for user: {UserId}, tokensProcessed: {TokenCount}", userId, activeAccessTokens.Count);
        
        logger.LogDebug("Completed {MethodName}", nameof(LogoutAll));
    }
}
