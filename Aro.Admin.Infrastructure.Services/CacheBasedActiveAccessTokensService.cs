using Aro.Admin.Application.Services;
using Aro.Admin.Application.Services.DTOs.ServiceResponses;
using Aro.Admin.Application.Shared.Options;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Aro.Admin.Infrastructure.Services;

public partial class CacheBasedActiveAccessTokensService(IDistributedCache cache, ISerializer serializer, IOptions<JwtOptions> jwtOptions, ILogManager<CacheBasedActiveAccessTokensService> logger) : IActiveAccessTokenService
{
    private readonly JwtOptions jwtOptions = jwtOptions.Value;

    public async Task RegisterToken(Guid userId, string tokenIdentifier, DateTime expiry, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Starting {MethodName}", nameof(RegisterToken));
        
        var key = GetKey(userId);
        logger.LogDebug("Retrieving existing active tokens for user: {UserId}", userId);
        var existingTokens = await GetActiveTokens(userId, cancellationToken).ConfigureAwait(false);
        logger.LogDebug("Found {ExistingTokenCount} existing tokens for user: {UserId}", existingTokens.Count, userId);
        
        existingTokens.Add(new TokenInfo(tokenIdentifier, expiry));
        logger.LogDebug("Added new token to existing tokens for user: {UserId}, totalTokens: {TotalTokens}", userId, existingTokens.Count);

        var serialized = serializer.Serialize(existingTokens);
        logger.LogDebug("Serialized tokens for user: {UserId}", userId);
        
        var cacheExpiry = TimeSpan.FromMinutes(jwtOptions.AccessTokenExpirationMinutes).Add(TimeSpan.FromMinutes(jwtOptions.AccessTokenTrackingMarginInMinutes));
        logger.LogDebug("Setting cache expiry for user: {UserId}, expiryMinutes: {ExpiryMinutes}", userId, cacheExpiry.TotalMinutes);
        
        await cache.SetStringAsync(key, serialized, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = cacheExpiry,
        }, cancellationToken).ConfigureAwait(false);
        
        logger.LogInfo("Successfully registered active access token for user: {UserId}, tokenIdentifier: {TokenIdentifier}", userId, tokenIdentifier);
        
        logger.LogDebug("Completed {MethodName}", nameof(RegisterToken));
    }

    public async Task<List<TokenInfo>> GetActiveTokens(Guid userId, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Starting {MethodName}", nameof(GetActiveTokens));
        
        var key = GetKey(userId);
        logger.LogDebug("Retrieving tokens from cache with key: {Key}", key);
        var serialized = await cache.GetStringAsync(key, cancellationToken);
        
        if (serialized == null)
        {
            logger.LogDebug("No active tokens found for user: {UserId}", userId);
            return [];
        }

        logger.LogDebug("Deserializing tokens for user: {UserId}", userId);
        var tokens = serializer.Deserialize<List<TokenInfo>>(serialized) ?? [];
        logger.LogDebug("Retrieved {TokenCount} active tokens for user: {UserId}", tokens.Count, userId);
        
        logger.LogDebug("Completed {MethodName}", nameof(GetActiveTokens));
        return tokens;
    }

    public async Task RemoveToken(Guid userId, string tokenIdentifier, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Starting {MethodName}", nameof(RemoveToken));
        
        logger.LogDebug("Getting current active tokens for user: {UserId}", userId);
        var tokens = await GetActiveTokens(userId, cancellationToken);
        logger.LogDebug("Found {TokenCount} tokens before removal for user: {UserId}", tokens.Count, userId);
        
        var removedCount = tokens.RemoveAll(t => t.TokenIdentifier == tokenIdentifier);
        logger.LogDebug("Removed {RemovedCount} tokens for user: {UserId}, remainingTokens: {RemainingTokens}", removedCount, userId, tokens.Count);
        
        var key = GetKey(userId);
        logger.LogDebug("Serializing and updating cache for user: {UserId}", userId);
        await cache.SetStringAsync(key, serializer.Serialize(tokens), cancellationToken).ConfigureAwait(false);
        
        logger.LogInfo("Successfully removed active access token for user: {UserId}, tokenIdentifier: {TokenIdentifier}, removedCount: {RemovedCount}", userId, tokenIdentifier, removedCount);
        
        logger.LogDebug("Completed {MethodName}", nameof(RemoveToken));
    }

    public async Task RemoveAllTokens(Guid userId, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Starting {MethodName}", nameof(RemoveAllTokens));
        
        var key = GetKey(userId);
        logger.LogDebug("Removing all tokens from cache with key: {Key} for user: {UserId}", key, userId);
        await cache.RemoveAsync(key, cancellationToken).ConfigureAwait(false);
        
        logger.LogInfo("Successfully removed all active access tokens for user: {UserId}", userId);
        
        logger.LogDebug("Completed {MethodName}", nameof(RemoveAllTokens));
    }
}