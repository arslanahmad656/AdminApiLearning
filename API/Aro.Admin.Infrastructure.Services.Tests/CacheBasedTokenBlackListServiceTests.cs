using Aro.Admin.Tests.Common;
using Aro.Common.Application.Services.LogManager;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using Xunit;

namespace Aro.Admin.Infrastructure.Services.Tests;

public class CacheBasedTokenBlackListServiceTests : TestBase
{
    private readonly Mock<IDistributedCache> mockCache;
    private readonly Mock<ILogManager<CacheBasedTokenBlackListService>> mockLogger;
    private readonly CacheBasedTokenBlackListService service;

    public CacheBasedTokenBlackListServiceTests()
    {
        mockCache = new Mock<IDistributedCache>();
        mockLogger = new Mock<ILogManager<CacheBasedTokenBlackListService>>();

        service = new CacheBasedTokenBlackListService(
            mockCache.Object,
            mockLogger.Object
        );
    }

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateInstance()
    {
        service.Should().NotBeNull();
    }

    [Fact]
    public async Task BlackList_WithValidTokenAndFutureExpiry_ShouldSetTokenInCache()
    {
        var tokenIdentifier = "test-token-123";
        var expiry = DateTime.UtcNow.AddHours(1);

        await service.BlackList(tokenIdentifier, expiry);

        mockCache.Verify(x => x.SetAsync(
            tokenIdentifier,
            It.IsAny<byte[]>(),
            It.Is<DistributedCacheEntryOptions>(options => 
                options.AbsoluteExpirationRelativeToNow.HasValue),
            It.IsAny<CancellationToken>()), Times.Once);
        
        mockLogger.Verify(x => x.LogDebug("Starting {MethodName}", nameof(CacheBasedTokenBlackListService.BlackList)), Times.Once);
        mockLogger.Verify(x => x.LogInfo("Successfully blacklisted token: {TokenIdentifier}, ttl: {Ttl}", tokenIdentifier, It.IsAny<TimeSpan>()), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Completed {MethodName}", nameof(CacheBasedTokenBlackListService.BlackList)), Times.Once);
    }

    [Fact]
    public async Task BlackList_WithNegativeTtl_ShouldSkipCacheEntry()
    {
        var tokenIdentifier = "expired-token";
        var expiry = DateTime.UtcNow.AddHours(-1);

        await service.BlackList(tokenIdentifier, expiry);

        mockCache.Verify(x => x.SetAsync(
            It.IsAny<string>(),
            It.IsAny<byte[]>(),
            It.IsAny<DistributedCacheEntryOptions>(),
            It.IsAny<CancellationToken>()), Times.Never);
        
        mockLogger.Verify(x => x.LogDebug("TTL is zero or negative for token: {TokenIdentifier}, skipping cache entry", tokenIdentifier), Times.Once);
    }

    [Fact]
    public async Task BlackList_WithZeroTtl_ShouldSkipCacheEntry()
    {
        var tokenIdentifier = "zero-ttl-token";
        var expiry = DateTime.UtcNow;

        await service.BlackList(tokenIdentifier, expiry);

        mockCache.Verify(x => x.SetAsync(
            It.IsAny<string>(),
            It.IsAny<byte[]>(),
            It.IsAny<DistributedCacheEntryOptions>(),
            It.IsAny<CancellationToken>()), Times.Never);
        
        mockLogger.Verify(x => x.LogDebug("TTL is zero or negative for token: {TokenIdentifier}, skipping cache entry", tokenIdentifier), Times.Once);
    }

    [Fact]
    public async Task BlackList_WithCancellationToken_ShouldPassTokenToCache()
    {
        var tokenIdentifier = "test-token";
        var expiry = DateTime.UtcNow.AddHours(1);
        var cancellationToken = new CancellationToken();

        await service.BlackList(tokenIdentifier, expiry, cancellationToken);

        mockCache.Verify(x => x.SetAsync(
            tokenIdentifier,
            It.IsAny<byte[]>(),
            It.IsAny<DistributedCacheEntryOptions>(),
            cancellationToken), Times.Once);
    }

    [Fact]
    public async Task IsBlackListed_WithBlacklistedToken_ShouldReturnTrue()
    {
        var tokenIdentifier = "blacklisted-token";
        var revokedBytes = System.Text.Encoding.UTF8.GetBytes("revoked");
        mockCache.Setup(x => x.GetAsync(tokenIdentifier, It.IsAny<CancellationToken>()))
            .ReturnsAsync(revokedBytes);

        var result = await service.IsBlackListed(tokenIdentifier);

        result.Should().BeTrue();
        mockCache.Verify(x => x.GetAsync(tokenIdentifier, It.IsAny<CancellationToken>()), Times.Once);
        
        mockLogger.Verify(x => x.LogDebug("Starting {MethodName}", nameof(CacheBasedTokenBlackListService.IsBlackListed)), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Token blacklist check completed: {TokenIdentifier}, isBlacklisted: {IsBlacklisted}", tokenIdentifier, true), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Completed {MethodName}", nameof(CacheBasedTokenBlackListService.IsBlackListed)), Times.Once);
    }

    [Fact]
    public async Task IsBlackListed_WithNonBlacklistedToken_ShouldReturnFalse()
    {
        var tokenIdentifier = "valid-token";
        mockCache.Setup(x => x.GetAsync(tokenIdentifier, It.IsAny<CancellationToken>()))
            .ReturnsAsync((byte[]?)null);

        var result = await service.IsBlackListed(tokenIdentifier);

        result.Should().BeFalse();
        mockCache.Verify(x => x.GetAsync(tokenIdentifier, It.IsAny<CancellationToken>()), Times.Once);
        
        mockLogger.Verify(x => x.LogDebug("Starting {MethodName}", nameof(CacheBasedTokenBlackListService.IsBlackListed)), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Token blacklist check completed: {TokenIdentifier}, isBlacklisted: {IsBlacklisted}", tokenIdentifier, false), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Completed {MethodName}", nameof(CacheBasedTokenBlackListService.IsBlackListed)), Times.Once);
    }

    [Fact]
    public async Task IsBlackListed_WithEmptyArray_ShouldReturnTrue()
    {
        var tokenIdentifier = "empty-token";
        mockCache.Setup(x => x.GetAsync(tokenIdentifier, It.IsAny<CancellationToken>()))
            .ReturnsAsync(System.Text.Encoding.UTF8.GetBytes(string.Empty));

        var result = await service.IsBlackListed(tokenIdentifier);

        result.Should().BeTrue();
        mockCache.Verify(x => x.GetAsync(tokenIdentifier, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task IsBlackListed_WithCancellationToken_ShouldPassTokenToCache()
    {
        var tokenIdentifier = "test-token";
        var cancellationToken = new CancellationToken();
        mockCache.Setup(x => x.GetAsync(tokenIdentifier, cancellationToken))
            .ReturnsAsync((byte[]?)null);

        var result = await service.IsBlackListed(tokenIdentifier, cancellationToken);

        result.Should().BeFalse();
        mockCache.Verify(x => x.GetAsync(tokenIdentifier, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task BlackList_WithLongTtl_ShouldSetCorrectTtl()
    {
        var tokenIdentifier = "long-lived-token";
        var expiry = DateTime.UtcNow.AddDays(7);

        await service.BlackList(tokenIdentifier, expiry);

        mockCache.Verify(x => x.SetAsync(
            tokenIdentifier,
            It.IsAny<byte[]>(),
            It.Is<DistributedCacheEntryOptions>(options => 
                options.AbsoluteExpirationRelativeToNow.HasValue &&
                options.AbsoluteExpirationRelativeToNow.Value > TimeSpan.FromDays(6)),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task BlackList_WithVeryShortTtl_ShouldSetCorrectTtl()
    {
        var tokenIdentifier = "short-lived-token";
        var expiry = DateTime.UtcNow.AddSeconds(30);

        await service.BlackList(tokenIdentifier, expiry);

        mockCache.Verify(x => x.SetAsync(
            tokenIdentifier,
            It.IsAny<byte[]>(),
            It.Is<DistributedCacheEntryOptions>(options => 
                options.AbsoluteExpirationRelativeToNow.HasValue &&
                options.AbsoluteExpirationRelativeToNow.Value <= TimeSpan.FromMinutes(1)),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task IsBlackListed_WithWhitespaceValue_ShouldReturnTrue()
    {
        var tokenIdentifier = "whitespace-token";
        var whitespaceBytes = System.Text.Encoding.UTF8.GetBytes("   ");
        mockCache.Setup(x => x.GetAsync(tokenIdentifier, It.IsAny<CancellationToken>()))
            .ReturnsAsync(whitespaceBytes);

        var result = await service.IsBlackListed(tokenIdentifier);

        result.Should().BeTrue();
        mockCache.Verify(x => x.GetAsync(tokenIdentifier, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task BlackList_ShouldLogCorrectCacheOptions()
    {
        var tokenIdentifier = "logging-test-token";
        var expiry = DateTime.UtcNow.AddMinutes(30);

        await service.BlackList(tokenIdentifier, expiry);

        mockLogger.Verify(x => x.LogDebug("Setting cache options for token: {TokenIdentifier}, absoluteExpiry: {AbsoluteExpiry}", tokenIdentifier, It.IsAny<TimeSpan>()), Times.Once);
    }

    [Fact]
    public async Task BlackList_WithNegativeTtl_ShouldLogSkippingMessage()
    {
        var tokenIdentifier = "negative-ttl-token";
        var expiry = DateTime.UtcNow.AddMinutes(-5);

        await service.BlackList(tokenIdentifier, expiry);

        mockLogger.Verify(x => x.LogDebug("TTL is zero or negative for token: {TokenIdentifier}, skipping cache entry", tokenIdentifier), Times.Once);
        mockCache.Verify(x => x.SetAsync(
            It.IsAny<string>(),
            It.IsAny<byte[]>(),
            It.IsAny<DistributedCacheEntryOptions>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task BlackList_ShouldSetCorrectValue()
    {
        var tokenIdentifier = "value-test-token";
        var expiry = DateTime.UtcNow.AddHours(1);

        await service.BlackList(tokenIdentifier, expiry);

        mockCache.Verify(x => x.SetAsync(
            tokenIdentifier,
            It.Is<byte[]>(bytes => System.Text.Encoding.UTF8.GetString(bytes) == "revoked"),
            It.IsAny<DistributedCacheEntryOptions>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}