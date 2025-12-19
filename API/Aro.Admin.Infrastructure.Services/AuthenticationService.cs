using Aro.Admin.Application.Repository;
using Aro.Admin.Application.Services.AccessToken;
using Aro.Admin.Application.Services.AccountLockout;
using Aro.Admin.Application.Services.Authentication;
using Aro.Admin.Application.Services.Hasher;
using Aro.Admin.Application.Services.User;
using Aro.Common.Application.Repository;
using Aro.Common.Application.Services.LogManager;
using Aro.Common.Application.Services.RequestInterpretor;
using Aro.Common.Application.Services.SystemContext;
using Aro.Common.Application.Services.UniqueIdGenerator;
using Aro.Common.Domain.Shared;

namespace Aro.Admin.Infrastructure.Services;

public partial class AuthenticationService(IHasher haser, IUserService userService, IAccessTokenService accessTokenService, IRefreshTokenService refreshTokenService,
    Application.Repository.IRepositoryManager repositoryManager, IUnitOfWork unitOfWork, IUniqueIdGenerator idGenerator, ITokenBlackListService tokenBlackListService,
    IActiveAccessTokenService activeAccessTokenService, ErrorCodes errorCodes, ILogManager<AuthenticationService> logger, IRequestInterpretorService currentUserService,
    ISystemContextFactory systemContextFactory, IAccountLockoutService accountLockoutService) : IAuthenticationService
{
    private readonly IRefreshTokenRepository refreshTokenRepo = repositoryManager.RefreshTokenRepository;

    public async Task<CompositeToken> Authenticate(string email, string password, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Starting {MethodName}", nameof(Authenticate));

        if (!currentUserService.IsAuthenticated())
        {
            using var systemContext = systemContextFactory.Create();
            return await AuthenticateInternal(email, password, cancellationToken).ConfigureAwait(false);
        }

        return await AuthenticateInternal(email, password, cancellationToken).ConfigureAwait(false);
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
