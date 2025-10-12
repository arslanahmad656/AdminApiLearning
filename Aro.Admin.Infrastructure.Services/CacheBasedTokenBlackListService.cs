using Aro.Admin.Application.Services;
using Microsoft.Extensions.Caching.Distributed;

namespace Aro.Admin.Infrastructure.Services;

public class CacheBasedTokenBlackListService(IDistributedCache cache) : ITokenBlackListService
{
    public async Task BlackList(string tokenIdentifier, DateTime expiry, CancellationToken cancellationToken = default)
    {
        var ttl = expiry - DateTime.UtcNow;
        if (ttl < TimeSpan.Zero)
        {
            ttl = TimeSpan.Zero;
        }

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = ttl
        };

        await cache.SetStringAsync(tokenIdentifier, "revoked", options, cancellationToken);
    }

    public async Task<bool> IsBlackListed(string tokenIdentifier, CancellationToken cancellationToken = default)
    {
        var value = await cache.GetStringAsync(tokenIdentifier, cancellationToken);
        return value != null;
    }
}
