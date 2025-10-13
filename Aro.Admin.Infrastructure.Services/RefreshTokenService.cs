using Aro.Admin.Application.Services;
using Aro.Admin.Application.Services.DTOs.ServiceResponses;
using Aro.Admin.Application.Shared.Options;
using Aro.Admin.Domain.Repository;
using Aro.Admin.Domain.Shared.Exceptions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using RefreshToken = Aro.Admin.Application.Services.DTOs.ServiceResponses.RefreshToken;

namespace Aro.Admin.Infrastructure.Services;

public partial class RefreshTokenService(IOptions<JwtOptions> jwtOptions, IRepositoryManager repositoryManager, IMapper mapper, IHasher hasher, IAccessTokenService accessTokenService, IUniqueIdGenerator idGenerator, ErrorCodes errorCodes, ILogManager<RefreshTokenService> logger) : IRefreshTokenService
{
    private readonly JwtOptions jwtOptions = jwtOptions.Value;
    private readonly IRefreshTokenRepository refreshTokenRepo = repositoryManager.RefreshTokenRepository;

    public Task<RefreshToken> GenerateRefreshToken(CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Starting {MethodName}", nameof(GenerateRefreshToken));
        
        var bytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        logger.LogDebug("Generated random bytes for refresh token");

        var rawToken = Convert.ToBase64String(bytes);
        var expiry = DateTime.UtcNow.AddHours(jwtOptions.RefreshTokenExpirationHours);
        logger.LogDebug("Created refresh token with expiry: {Expiry}", expiry);

        var token = new RefreshToken(rawToken, expiry);
        logger.LogDebug("Refresh token generated successfully, expiry: {Expiry}", expiry);

        logger.LogDebug("Completed {MethodName}", nameof(GenerateRefreshToken));
        return Task.FromResult(token);
    }

    public async Task<UserRefreshToken> GetActiveToken(Guid userId, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Starting {MethodName}", nameof(GetActiveToken));
        
        var query = refreshTokenRepo.GetActiveTokensByUserId(userId);
        logger.LogDebug("Retrieved active tokens query for user: {UserId}", userId);

        var tokenEntity = await query
            .OrderByDescending(rt => rt.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false)
            ?? throw new AroRefreshTokenNotFoundException(userId.ToString(), AroRefreshTokenNotFoundException.IdentifierType.User);

        logger.LogDebug("Found active refresh token for user: {UserId}, tokenId: {TokenId}", userId, tokenEntity.Id);

        var token = mapper.Map<UserRefreshToken>(tokenEntity);
        logger.LogDebug("Mapped refresh token entity to response for user: {UserId}", userId);

        logger.LogDebug("Completed {MethodName}", nameof(GetActiveToken));
        return token;
    }
    
    public async Task<UserRefreshToken> GetActiveToken(string refreshToken, CancellationToken cancellationToken = default)
    { 
        var tokenHash = hasher.Hash(refreshToken);
        var query = refreshTokenRepo.GetActiveTokenByTokenHash(tokenHash);

        var tokenEntity = await query
            .OrderByDescending(rt => rt.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false)
            ?? throw new AroRefreshTokenNotFoundException(tokenHash, AroRefreshTokenNotFoundException.IdentifierType.Token);

        var token = mapper.Map<UserRefreshToken>(tokenEntity);

        return token;
    }

    public async Task RevokeAll(Guid userId, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Starting {MethodName}", nameof(RevokeAll));
        
        var query = refreshTokenRepo.GetActiveTokensByUserId(userId);
        logger.LogDebug("Retrieved active tokens query for user: {UserId}", userId);

        var tokens = await query.ToListAsync(cancellationToken).ConfigureAwait(false);
        logger.LogDebug("Found {TokenCount} active tokens for user: {UserId}", tokens.Count, userId);

        var now = DateTime.UtcNow;
        tokens.ForEach(t => MarkRevoked(t, now));
        logger.LogDebug("Marked {TokenCount} tokens as revoked for user: {UserId}", tokens.Count, userId);

        await repositoryManager.SaveChanges(cancellationToken).ConfigureAwait(false);
        logger.LogInfo("Successfully revoked all refresh tokens for user: {UserId}, tokenCount: {TokenCount}", userId, tokens.Count);
        
        logger.LogDebug("Completed {MethodName}", nameof(RevokeAll));
    }

    public async Task Revoke(Guid tokenId, CancellationToken cancellationToken = default)
    {
        var query = refreshTokenRepo.GetActiveTokenById(tokenId);

        var token = await query.FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false)
            ?? throw new AroRefreshTokenNotFoundException(tokenId.ToString(), AroRefreshTokenNotFoundException.IdentifierType.Token);

        MarkRevoked(token, DateTime.Now);

        await repositoryManager.SaveChanges(cancellationToken).ConfigureAwait(false);
    }

    public async Task<bool> Revoke(Guid userId, string refreshToken, CancellationToken cancellationToken = default)
    {
        var tokenHash = hasher.Hash(refreshToken);
        var query = refreshTokenRepo.GetActiveTokensByUserAndTokenHash(userId, tokenHash);
        var token = await query.FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);

        if (token is null)
        {
            // refresh token was invalid or the token was already expired or some other problem.
            // In any case, we don't need to do anything. This operation can fail silently and just indicate the result.

            return false;
        }

        MarkRevoked(token, DateTime.Now);
        await repositoryManager.SaveChanges(cancellationToken).ConfigureAwait(false);

        return true;
    }

    public async Task<CompositeToken> RefreshToken(string refreshToken, CancellationToken cancellationToken = default)
    {
        var token = await GetActiveToken(refreshToken, cancellationToken).ConfigureAwait(false);

        var compositeToken = await RefreshToken(token.UserId, refreshToken, cancellationToken).ConfigureAwait(false);

        return compositeToken;
    }

    public async Task<CompositeToken> RefreshToken(Guid userId, string refreshToken, CancellationToken cancellationToken = default)
    {
        var existingRefreshToken = await refreshTokenRepo
            .GetActiveTokensByUserId(userId)
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false)
            ?? throw new AroRefreshTokenNotFoundException(userId.ToString(), AroRefreshTokenNotFoundException.IdentifierType.User);

        if (!hasher.Verify(refreshToken, existingRefreshToken.TokenHash))
        {
            throw new AroUnauthorizedException(errorCodes.INVALID_REFRESH_TOKEN, $"The refresh token does not match.");
        }

        var now = DateTime.Now;

        MarkRevoked(existingRefreshToken, now);

        var newAccessToken = await accessTokenService.GenerateAccessToken(userId, cancellationToken).ConfigureAwait(false);

        var newRefreshToken = await GenerateRefreshToken(cancellationToken).ConfigureAwait(false);

        var newRefreshEntity = new Domain.Entities.RefreshToken
        {
            Id = idGenerator.Generate(),
            UserId = userId,
            TokenHash = hasher.Hash(newRefreshToken.Token),
            CreatedAt = now,
            ExpiresAt = newRefreshToken.ExpiresAt,
        };

        await refreshTokenRepo.Create(newRefreshEntity, cancellationToken).ConfigureAwait(false);

        await repositoryManager.SaveChanges(cancellationToken).ConfigureAwait(false);

        return new(existingRefreshToken.TokenHash, userId, newRefreshEntity.Id, newAccessToken.Token, newRefreshToken.Token, newAccessToken.Expiry, newRefreshToken.ExpiresAt, newAccessToken.TokenIdentifier);
    }
}
