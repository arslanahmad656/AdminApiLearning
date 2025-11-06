using Aro.Admin.Application.Services.AccessToken;
using Aro.Common.Application.Services.LogManager;
using Microsoft.Extensions.Caching.Distributed;

namespace Aro.Admin.Infrastructure.Services;

public class CacheBasedTokenBlackListService(IDistributedCache cache, ILogManager<CacheBasedTokenBlackListService> logger) : ITokenBlackListService
{
    public async Task BlackList(string tokenIdentifier, DateTime expiry, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Starting {MethodName}", nameof(BlackList));
        
        var ttl = expiry - DateTime.UtcNow;
        logger.LogDebug("Calculated TTL for token: {TokenIdentifier}, ttl: {Ttl}", tokenIdentifier, ttl);
        
        if (ttl <= TimeSpan.Zero)
        {
            logger.LogDebug("TTL is zero or negative for token: {TokenIdentifier}, skipping cache entry", tokenIdentifier);
            return;
        }

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = ttl
        };
        logger.LogDebug("Setting cache options for token: {TokenIdentifier}, absoluteExpiry: {AbsoluteExpiry}", tokenIdentifier, ttl);

        var value = System.Text.Encoding.UTF8.GetBytes("revoked");
        await cache.SetAsync(tokenIdentifier, value, options, cancellationToken);
        logger.LogInfo("Successfully blacklisted token: {TokenIdentifier}, ttl: {Ttl}", tokenIdentifier, ttl);
        
        logger.LogDebug("Completed {MethodName}", nameof(BlackList));
    }

    public async Task<bool> IsBlackListed(string tokenIdentifier, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Starting {MethodName}", nameof(IsBlackListed));
        
        var value = await cache.GetAsync(tokenIdentifier, cancellationToken);
        var isBlacklisted = value != null;
        logger.LogDebug("Token blacklist check completed: {TokenIdentifier}, isBlacklisted: {IsBlacklisted}", tokenIdentifier, isBlacklisted);
        
        logger.LogDebug("Completed {MethodName}", nameof(IsBlackListed));
        return isBlacklisted;
    }
}
