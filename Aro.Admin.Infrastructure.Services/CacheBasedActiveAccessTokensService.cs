using Aro.Admin.Application.Services;
using Aro.Admin.Application.Services.DTOs.ServiceResponses;
using Aro.Admin.Application.Shared.Options;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Aro.Admin.Infrastructure.Services;

public partial class CacheBasedActiveAccessTokensService(IDistributedCache cache, ISerializer serializer, JwtOptions jwtOptions) : IActiveAccessTokenService
{
    public async Task RegisterToken(Guid userId, string tokenIdentifier, DateTime expiry, CancellationToken cancellationToken = default)
    {
        var key = GetKey(userId);
        var existingTokens = await GetActiveTokens(userId, cancellationToken).ConfigureAwait(false);
        existingTokens.Add(new(tokenIdentifier, expiry));

        var serialized = serializer.Serialize(existingTokens);
        await cache.SetStringAsync(key, serialized, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(jwtOptions.AccessTokenExpirationMinutes).Add(TimeSpan.FromMinutes(jwtOptions.AccessTokenTrackingMarginInMinutes)),
        }, cancellationToken).ConfigureAwait(false);
    }

    public async Task<List<TokenInfo>> GetActiveTokens(Guid userId, CancellationToken cancellationToken = default)
    {
        var key = GetKey(userId);
        var serialized = await cache.GetStringAsync(key, cancellationToken);
        if (serialized == null) return [];

        return serializer.Deserialize<List<TokenInfo>>(serialized) ?? [];
    }

    public async Task RemoveToken(Guid userId, string tokenIdentifier, CancellationToken cancellationToken = default)
    {
        var tokens = await GetActiveTokens(userId, cancellationToken);
        tokens.RemoveAll(t => t.TokenIdentifier == tokenIdentifier);
        var key = GetKey(userId);
        await cache.SetStringAsync(key, serializer.Serialize(tokens), cancellationToken).ConfigureAwait(false);
    }

    public async Task RemoveAllTokens(Guid userId, CancellationToken cancellationToken = default)
    {
        var key = GetKey(userId);
        await cache.RemoveAsync(key, cancellationToken).ConfigureAwait(false);
    }
}