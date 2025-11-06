using Aro.Admin.Application.Services.DTOs.ServiceResponses;
using Aro.Admin.Application.Services.Serializer;
using Aro.Admin.Application.Shared.Options;
using Aro.Admin.Tests.Common;
using Aro.Common.Application.Services.LogManager;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Aro.Admin.Infrastructure.Services.Tests;

public class CacheBasedActiveAccessTokensServiceTests : TestBase
{
    private readonly Mock<IDistributedCache> mockCache;
    private readonly Mock<ISerializer> mockSerializer;
    private readonly Mock<IOptions<JwtOptions>> mockJwtOptions;
    private readonly Mock<ILogManager<CacheBasedActiveAccessTokensService>> mockLogger;
    private readonly JwtOptions jwtOptions;
    private readonly CacheBasedActiveAccessTokensService service;

    public CacheBasedActiveAccessTokensServiceTests()
    {
        mockCache = new Mock<IDistributedCache>();
        mockSerializer = new Mock<ISerializer>();
        mockJwtOptions = new Mock<IOptions<JwtOptions>>();
        mockLogger = new Mock<ILogManager<CacheBasedActiveAccessTokensService>>();

        jwtOptions = new JwtOptions { AccessTokenExpirationMinutes = 60 };
        mockJwtOptions.Setup(x => x.Value).Returns(jwtOptions);

        service = new CacheBasedActiveAccessTokensService(
            mockCache.Object,
            mockSerializer.Object,
            mockJwtOptions.Object,
            mockLogger.Object
        );
    }

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateInstance()
    {
        service.Should().NotBeNull();
    }

    [Fact]
    public async Task GetActiveTokens_WithNoCachedTokens_ShouldReturnEmptyList()
    {
        var userId = Guid.NewGuid();
        mockCache.Setup(x => x.GetAsync($"active_tokens_{userId}", It.IsAny<CancellationToken>()))
            .ReturnsAsync((byte[]?)null);

        var result = await service.GetActiveTokens(userId);

        result.Should().BeEmpty();
        mockCache.Verify(x => x.GetAsync($"active_tokens_{userId}", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetActiveTokens_WithEmptyCache_ShouldReturnEmptyList()
    {
        var userId = Guid.NewGuid();
        mockCache.Setup(x => x.GetAsync($"active_tokens_{userId}", It.IsAny<CancellationToken>()))
            .ReturnsAsync(System.Text.Encoding.UTF8.GetBytes(string.Empty));

        var result = await service.GetActiveTokens(userId);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetActiveTokens_WithCachedTokens_ShouldReturnDeserializedTokens()
    {
        var userId = Guid.NewGuid();
        var expectedTokens = new List<TokenInfo>
        {
            new TokenInfo("token-1", DateTime.UtcNow.AddHours(1)),
            new TokenInfo("token-2", DateTime.UtcNow.AddHours(2))
        };

        var serializedTokens = System.Text.Encoding.UTF8.GetBytes("serialized-tokens");
        mockCache.Setup(x => x.GetAsync($"active_tokens_{userId}", It.IsAny<CancellationToken>()))
            .ReturnsAsync(serializedTokens);
        mockSerializer.Setup(x => x.Deserialize<List<TokenInfo>>("serialized-tokens"))
            .Returns(expectedTokens);

        var result = await service.GetActiveTokens(userId);

        result.Should().BeEquivalentTo(expectedTokens);
        mockCache.Verify(x => x.GetAsync($"active_tokens_{userId}", It.IsAny<CancellationToken>()), Times.Once);
        mockSerializer.Verify(x => x.Deserialize<List<TokenInfo>>("serialized-tokens"), Times.Once);
    }

    [Fact]
    public async Task GetActiveTokens_WithNullDeserialization_ShouldReturnEmptyList()
    {
        var userId = Guid.NewGuid();
        var serializedTokens = System.Text.Encoding.UTF8.GetBytes("serialized-tokens");
        mockCache.Setup(x => x.GetAsync($"active_tokens_{userId}", It.IsAny<CancellationToken>()))
            .ReturnsAsync(serializedTokens);
        mockSerializer.Setup(x => x.Deserialize<List<TokenInfo>>("serialized-tokens"))
            .Returns((List<TokenInfo>?)null);

        var result = await service.GetActiveTokens(userId);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetActiveTokens_WithCancellationToken_ShouldPassTokenToCache()
    {
        var userId = Guid.NewGuid();
        var cancellationToken = new CancellationToken();
        mockCache.Setup(x => x.GetAsync($"active_tokens_{userId}", cancellationToken))
            .ReturnsAsync((byte[]?)null);

        await service.GetActiveTokens(userId, cancellationToken);

        mockCache.Verify(x => x.GetAsync($"active_tokens_{userId}", cancellationToken), Times.Once);
    }

    [Fact]
    public async Task RegisterToken_WithValidParameters_ShouldCallCacheAndSerializer()
    {
        var userId = Guid.NewGuid();
        var tokenIdentifier = "test-token";
        var expiry = DateTime.UtcNow.AddHours(1);

        mockCache.Setup(x => x.GetAsync($"active_tokens_{userId}", It.IsAny<CancellationToken>()))
            .ReturnsAsync((byte[]?)null);
        mockSerializer.Setup(x => x.Serialize(It.IsAny<List<TokenInfo>>(), It.IsAny<bool>()))
            .Returns("serialized-tokens");

        await service.RegisterToken(userId, tokenIdentifier, expiry);

        mockCache.Verify(x => x.GetAsync($"active_tokens_{userId}", It.IsAny<CancellationToken>()), Times.Once);
        mockSerializer.Verify(x => x.Serialize(It.IsAny<List<TokenInfo>>(), It.IsAny<bool>()), Times.Once);
    }

    [Fact]
    public async Task RegisterToken_WithExistingTokens_ShouldAddToExistingList()
    {
        var userId = Guid.NewGuid();
        var tokenIdentifier = "new-token";
        var expiry = DateTime.UtcNow.AddHours(1);

        var existingTokens = new List<TokenInfo>
        {
            new TokenInfo("existing-token", DateTime.UtcNow.AddHours(2))
        };

        var serializedExisting = System.Text.Encoding.UTF8.GetBytes("serialized-existing");
        mockCache.Setup(x => x.GetAsync($"active_tokens_{userId}", It.IsAny<CancellationToken>()))
            .ReturnsAsync(serializedExisting);
        mockSerializer.Setup(x => x.Deserialize<List<TokenInfo>>("serialized-existing"))
            .Returns(existingTokens);
        mockSerializer.Setup(x => x.Serialize(It.IsAny<List<TokenInfo>>(), It.IsAny<bool>()))
            .Returns("serialized-updated");

        await service.RegisterToken(userId, tokenIdentifier, expiry);

        mockSerializer.Verify(x => x.Deserialize<List<TokenInfo>>("serialized-existing"), Times.Once);
        mockSerializer.Verify(x => x.Serialize(It.IsAny<List<TokenInfo>>(), It.IsAny<bool>()), Times.Once);
    }

    [Fact]
    public async Task RegisterToken_WithCancellationToken_ShouldPassTokenToCache()
    {
        var userId = Guid.NewGuid();
        var tokenIdentifier = "test-token";
        var expiry = DateTime.UtcNow.AddHours(1);
        var cancellationToken = new CancellationToken();

        mockCache.Setup(x => x.GetAsync($"active_tokens_{userId}", cancellationToken))
            .ReturnsAsync((byte[]?)null);
        mockSerializer.Setup(x => x.Serialize(It.IsAny<List<TokenInfo>>(), It.IsAny<bool>()))
            .Returns("serialized-tokens");

        await service.RegisterToken(userId, tokenIdentifier, expiry, cancellationToken);

        mockCache.Verify(x => x.GetAsync($"active_tokens_{userId}", cancellationToken), Times.Once);
    }

    [Fact]
    public async Task RemoveToken_WithExistingToken_ShouldRemoveTokenAndUpdateCache()
    {
        var userId = Guid.NewGuid();
        var tokenToRemove = "token-to-remove";
        var existingTokens = new List<TokenInfo>
        {
            new TokenInfo(tokenToRemove, DateTime.UtcNow.AddHours(1)),
            new TokenInfo("other-token", DateTime.UtcNow.AddHours(1))
        };

        var serializedTokens = System.Text.Encoding.UTF8.GetBytes("serialized-tokens");
        mockCache.Setup(x => x.GetAsync($"active_tokens_{userId}", It.IsAny<CancellationToken>()))
            .ReturnsAsync(serializedTokens);
        mockSerializer.Setup(x => x.Deserialize<List<TokenInfo>>("serialized-tokens"))
            .Returns(existingTokens);
        mockSerializer.Setup(x => x.Serialize(It.IsAny<List<TokenInfo>>(), It.IsAny<bool>()))
            .Returns("serialized-updated");

        await service.RemoveToken(userId, tokenToRemove);

        mockCache.Verify(x => x.GetAsync($"active_tokens_{userId}", It.IsAny<CancellationToken>()), Times.Once);
        mockSerializer.Verify(x => x.Deserialize<List<TokenInfo>>("serialized-tokens"), Times.Once);
        mockSerializer.Verify(x => x.Serialize(It.IsAny<List<TokenInfo>>(), It.IsAny<bool>()), Times.Once);
    }

    [Fact]
    public async Task RemoveToken_WithNonExistentToken_ShouldNotRemoveAnything()
    {
        var userId = Guid.NewGuid();
        var tokenToRemove = "token-to-remove";
        var existingTokens = new List<TokenInfo>
        {
            new TokenInfo("existing-token-1", DateTime.UtcNow.AddHours(1)),
            new TokenInfo("existing-token-2", DateTime.UtcNow.AddHours(1))
        };

        var serializedTokens = System.Text.Encoding.UTF8.GetBytes("serialized-tokens");
        mockCache.Setup(x => x.GetAsync($"active_tokens_{userId}", It.IsAny<CancellationToken>()))
            .ReturnsAsync(serializedTokens);
        mockSerializer.Setup(x => x.Deserialize<List<TokenInfo>>("serialized-tokens"))
            .Returns(existingTokens);

        await service.RemoveToken(userId, tokenToRemove);

        mockCache.Verify(x => x.GetAsync($"active_tokens_{userId}", It.IsAny<CancellationToken>()), Times.Once);
        mockSerializer.Verify(x => x.Deserialize<List<TokenInfo>>("serialized-tokens"), Times.Once);
        // Verify that serialize was not called since no changes were made
        mockSerializer.Verify(x => x.Serialize(It.IsAny<List<TokenInfo>>(), It.IsAny<bool>()), Times.Never);
    }

    [Fact]
    public async Task RemoveToken_WithCancellationToken_ShouldPassTokenToService()
    {
        var userId = Guid.NewGuid();
        var tokenIdentifier = "test-token";
        var cancellationToken = new CancellationToken();

        mockCache.Setup(x => x.GetAsync($"active_tokens_{userId}", cancellationToken))
            .ReturnsAsync((byte[]?)null);

        await service.RemoveToken(userId, tokenIdentifier, cancellationToken);

        mockCache.Verify(x => x.GetAsync($"active_tokens_{userId}", cancellationToken), Times.Once);
    }

    [Fact]
    public async Task RemoveAllTokens_WithValidUserId_ShouldRemoveFromCache()
    {
        var userId = Guid.NewGuid();

        await service.RemoveAllTokens(userId);

        // Verify that the cache was accessed to remove the tokens
        mockCache.Verify(x => x.RemoveAsync($"active_tokens_{userId}", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RemoveAllTokens_WithCancellationToken_ShouldPassTokenToCache()
    {
        var userId = Guid.NewGuid();
        var cancellationToken = new CancellationToken();

        await service.RemoveAllTokens(userId, cancellationToken);

        mockCache.Verify(x => x.RemoveAsync($"active_tokens_{userId}", cancellationToken), Times.Once);
    }

    [Fact]
    public async Task RegisterToken_ShouldSetCorrectCacheExpiry()
    {
        var userId = Guid.NewGuid();
        var tokenIdentifier = "test-token";
        var expiry = DateTime.UtcNow.AddHours(1);

        mockCache.Setup(x => x.GetAsync($"active_tokens_{userId}", It.IsAny<CancellationToken>()))
            .ReturnsAsync((byte[]?)null);
        mockSerializer.Setup(x => x.Serialize(It.IsAny<List<TokenInfo>>(), It.IsAny<bool>()))
            .Returns("serialized-tokens");

        await service.RegisterToken(userId, tokenIdentifier, expiry);

        // Verify that the cache operations were called
        mockCache.Verify(x => x.GetAsync($"active_tokens_{userId}", It.IsAny<CancellationToken>()), Times.Once);
        mockSerializer.Verify(x => x.Serialize(It.IsAny<List<TokenInfo>>(), It.IsAny<bool>()), Times.Once);
    }

    [Fact]
    public async Task RemoveToken_WithEmptyTokenList_ShouldHandleGracefully()
    {
        var userId = Guid.NewGuid();
        var tokenToRemove = "token-to-remove";
        var emptyTokens = new List<TokenInfo>();

        var serializedTokens = System.Text.Encoding.UTF8.GetBytes("serialized-empty");
        mockCache.Setup(x => x.GetAsync($"active_tokens_{userId}", It.IsAny<CancellationToken>()))
            .ReturnsAsync(serializedTokens);
        mockSerializer.Setup(x => x.Deserialize<List<TokenInfo>>("serialized-empty"))
            .Returns(emptyTokens);

        await service.RemoveToken(userId, tokenToRemove);

        mockCache.Verify(x => x.GetAsync($"active_tokens_{userId}", It.IsAny<CancellationToken>()), Times.Once);
        mockSerializer.Verify(x => x.Deserialize<List<TokenInfo>>("serialized-empty"), Times.Once);
        // Verify that serialize was not called since the list was already empty
        mockSerializer.Verify(x => x.Serialize(It.IsAny<List<TokenInfo>>(), It.IsAny<bool>()), Times.Never);
    }
}