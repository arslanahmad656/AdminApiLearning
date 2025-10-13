using Aro.Admin.Application.Services;
using Microsoft.Extensions.Caching.Distributed;

namespace Aro.Admin.Infrastructure.Services;

public class CacheBasedTokenBlackListService(IDistributedCache cache, ILogManager<CacheBasedTokenBlackListService> logger) : ITokenBlackListService
{
    public async Task BlackList(string tokenIdentifier, DateTime expiry, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Starting {MethodName}", nameof(BlackList));
        
        var ttl = expiry - DateTime.UtcNow;
        logger.LogDebug("Calculated TTL for token: {TokenIdentifier}, ttl: {Ttl}", tokenIdentifier, ttl);
        
        if (ttl < TimeSpan.Zero)
        {
            logger.LogDebug("TTL is negative for token: {TokenIdentifier}, setting to zero", tokenIdentifier);
            ttl = TimeSpan.Zero;
        }

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = ttl
        };
        logger.LogDebug("Setting cache options for token: {TokenIdentifier}, absoluteExpiry: {AbsoluteExpiry}", tokenIdentifier, ttl);

        await cache.SetStringAsync(tokenIdentifier, "revoked", options, cancellationToken);
        logger.LogInfo("Successfully blacklisted token: {TokenIdentifier}, ttl: {Ttl}", tokenIdentifier, ttl);
        
        logger.LogDebug("Completed {MethodName}", nameof(BlackList));
    }

    public async Task<bool> IsBlackListed(string tokenIdentifier, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Starting {MethodName}", nameof(IsBlackListed));
        
        var value = await cache.GetStringAsync(tokenIdentifier, cancellationToken);
        var isBlacklisted = value != null;
        logger.LogDebug("Token blacklist check completed: {TokenIdentifier}, isBlacklisted: {IsBlacklisted}", tokenIdentifier, isBlacklisted);
        
        logger.LogDebug("Completed {MethodName}", nameof(IsBlackListed));
        return isBlacklisted;
    }
}
