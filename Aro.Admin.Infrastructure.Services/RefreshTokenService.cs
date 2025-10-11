using Aro.Admin.Application.Services;
using Aro.Admin.Application.Services.DTOs.ServiceResponses;
using Aro.Admin.Application.Shared.Options;
using Aro.Admin.Domain.Repository;
using Aro.Admin.Domain.Shared.Exceptions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using RefreshToken = Aro.Admin.Application.Services.DTOs.ServiceResponses.RefreshToken;

namespace Aro.Admin.Infrastructure.Services;

public partial class RefreshTokenService(JwtOptions jwtOptions, IRepositoryManager repositoryManager, IMapper mapper, IHasher hasher, IAccessTokenService accessTokenService, IUniqueIdGenerator idGenerator, ErrorCodes errorCodes) : IRefreshTokenService
{
    private readonly IRefreshTokenRepository refreshTokenRepo = repositoryManager.RefreshTokenRepository;

    public Task<RefreshToken> GenerateRefreshToken(CancellationToken cancellationToken = default)
    {
        var bytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);

        var rawToken = Convert.ToBase64String(bytes);
        var expiry = DateTime.UtcNow.AddHours(jwtOptions.RefreshTokenExpirationHours);

        var token = new RefreshToken(rawToken, expiry);

        return Task.FromResult(token);
    }

    public async Task<RefreshToken> GetActiveToken(Guid userId, CancellationToken cancellationToken = default)
    {
        var query = refreshTokenRepo.GetActiveTokensByUserId(userId);

        var tokenEntity = await query.OrderByDescending(rt => rt.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false)
            ?? throw new AroRefreshTokenNotFoundException(userId.ToString(), AroRefreshTokenNotFoundException.IdentifierType.User);

        var token = mapper.Map<RefreshToken>(tokenEntity);

        return token;
    }

    public async Task RevokeAll(Guid userId, CancellationToken cancellationToken = default)
    {
        var query = refreshTokenRepo.GetActiveTokensByUserId(userId);

        var tokens = await query.ToListAsync(cancellationToken).ConfigureAwait(false);

        var now = DateTime.UtcNow;
        tokens.ForEach(t => MarkRevoked(t, now));

        await repositoryManager.SaveChanges(cancellationToken).ConfigureAwait(false);
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

        return new(newRefreshEntity.Id, newAccessToken.Token, newRefreshToken.Token, newAccessToken.Expiry, newRefreshToken.ExpiresAt);
    }
}
