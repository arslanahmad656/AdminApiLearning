using Aro.Admin.Application.Services;
using Aro.Admin.Application.Services.DTOs.ServiceResponses;
using Aro.Admin.Application.Shared.Options;
using Aro.Admin.Domain.Repository;
using Aro.Admin.Domain.Shared.Exceptions;
using Aro.Admin.Tests.Common;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Aro.Admin.Infrastructure.Services.Tests;

public class RefreshTokenServiceTests : TestBase
{
    private readonly Mock<IOptions<JwtOptions>> mockJwtOptions;
    private readonly Mock<IRepositoryManager> mockRepositoryManager;
    private readonly Mock<IRefreshTokenRepository> mockRefreshTokenRepository;
    private readonly Mock<IHasher> mockHasher;
    private readonly Mock<IAccessTokenService> mockAccessTokenService;
    private readonly Mock<IUniqueIdGenerator> mockIdGenerator;
    private readonly Mock<ILogManager<RefreshTokenService>> mockLogger;
    private readonly JwtOptions jwtOptions;
    private readonly ErrorCodes errorCodes;
    private readonly RefreshTokenService service;

    public RefreshTokenServiceTests()
    {
        mockJwtOptions = new Mock<IOptions<JwtOptions>>();
        mockRepositoryManager = new Mock<IRepositoryManager>();
        mockRefreshTokenRepository = new Mock<IRefreshTokenRepository>();
        mockHasher = new Mock<IHasher>();
        mockAccessTokenService = new Mock<IAccessTokenService>();
        mockIdGenerator = new Mock<IUniqueIdGenerator>();
        mockLogger = new Mock<ILogManager<RefreshTokenService>>();

        jwtOptions = new JwtOptions
        {
            RefreshTokenExpirationHours = 24,
            AccessTokenExpirationMinutes = 60
        };

        errorCodes = new ErrorCodes();

        mockJwtOptions.Setup(x => x.Value).Returns(jwtOptions);
        mockRepositoryManager.Setup(x => x.RefreshTokenRepository).Returns(mockRefreshTokenRepository.Object);

        service = new RefreshTokenService(
            mockJwtOptions.Object,
            mockRepositoryManager.Object,
            mockHasher.Object,
            mockAccessTokenService.Object,
            mockIdGenerator.Object,
            errorCodes,
            mockLogger.Object
        );
    }

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateInstance()
    {
        service.Should().NotBeNull();
    }

    [Fact]
    public async Task GenerateRefreshToken_ShouldReturnRefreshTokenWithCorrectExpiry()
    {
        var result = await service.GenerateRefreshToken();

        result.Should().NotBeNull();
        result.Token.Should().NotBeNullOrEmpty();
        result.ExpiresAt.Should().BeCloseTo(DateTime.UtcNow.AddHours(jwtOptions.RefreshTokenExpirationHours), TimeSpan.FromMinutes(1));
    }

    [Fact]
    public async Task GenerateRefreshToken_WithCancellationToken_ShouldPassTokenToMethod()
    {
        var cancellationToken = new CancellationToken();
        
        var result = await service.GenerateRefreshToken(cancellationToken);

        result.Should().NotBeNull();
        result.Token.Should().NotBeNullOrEmpty();
        result.ExpiresAt.Should().BeCloseTo(DateTime.UtcNow.AddHours(jwtOptions.RefreshTokenExpirationHours), TimeSpan.FromMinutes(1));
    }

    [Fact]
    public async Task GenerateRefreshToken_ShouldGenerateUniqueTokens()
    {
        var result1 = await service.GenerateRefreshToken();
        var result2 = await service.GenerateRefreshToken();

        result1.Token.Should().NotBe(result2.Token);
    }

    [Fact]
    public async Task GenerateRefreshToken_ShouldLogAppropriateMessages()
    {
        var result = await service.GenerateRefreshToken();

        result.Should().NotBeNull();
        mockLogger.Verify(x => x.LogDebug("Starting {MethodName}", nameof(RefreshTokenService.GenerateRefreshToken)), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Generated random bytes for refresh token"), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Created refresh token with expiry: {Expiry}", It.IsAny<DateTime>()), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Refresh token generated successfully, expiry: {Expiry}", It.IsAny<DateTime>()), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Completed {MethodName}", nameof(RefreshTokenService.GenerateRefreshToken)), Times.Once);
    }

    [Fact]
    public async Task GenerateRefreshToken_ShouldGenerateTokenWithCorrectLength()
    {
        var result = await service.GenerateRefreshToken();

        result.Should().NotBeNull();
        result.Token.Should().NotBeNullOrEmpty();
        
        // Base64 encoded 64 bytes should be 88 characters
        result.Token.Length.Should().Be(88);
    }

    [Fact]
    public async Task GenerateRefreshToken_ShouldGenerateTokenWithValidBase64()
    {
        var result = await service.GenerateRefreshToken();

        result.Should().NotBeNull();
        result.Token.Should().NotBeNullOrEmpty();
        
        // Should be valid base64
        Action act = () => Convert.FromBase64String(result.Token);
        act.Should().NotThrow();
    }

    [Fact]
    public async Task GenerateRefreshToken_ShouldUseCorrectExpiryTime()
    {
        var result = await service.GenerateRefreshToken();

        result.Should().NotBeNull();
        result.ExpiresAt.Should().BeCloseTo(DateTime.UtcNow.AddHours(24), TimeSpan.FromMinutes(1));
    }

    [Fact]
    public async Task GenerateRefreshToken_ShouldGenerateDifferentTokensOnMultipleCalls()
    {
        var results = new List<RefreshToken>();
        
        for (int i = 0; i < 5; i++)
        {
            results.Add(await service.GenerateRefreshToken());
        }

        results.Should().HaveCount(5);
        results.Select(r => r.Token).Should().OnlyHaveUniqueItems();
    }

    [Fact]
    public async Task GenerateRefreshToken_ShouldHaveConsistentExpiryAcrossCalls()
    {
        var result1 = await service.GenerateRefreshToken();
        var result2 = await service.GenerateRefreshToken();

        result1.ExpiresAt.Should().BeCloseTo(result2.ExpiresAt, TimeSpan.FromMinutes(1));
    }

    [Fact]
    public async Task GenerateRefreshToken_ShouldUseJwtOptionsForExpiry()
    {
        // Arrange
        var customJwtOptions = new JwtOptions
        {
            RefreshTokenExpirationHours = 48
        };
        mockJwtOptions.Setup(x => x.Value).Returns(customJwtOptions);

        var customService = new RefreshTokenService(
            mockJwtOptions.Object,
            mockRepositoryManager.Object,
            mockHasher.Object,
            mockAccessTokenService.Object,
            mockIdGenerator.Object,
            errorCodes,
            mockLogger.Object
        );

        // Act
        var result = await customService.GenerateRefreshToken();

        // Assert
        result.Should().NotBeNull();
        result.ExpiresAt.Should().BeCloseTo(DateTime.UtcNow.AddHours(48), TimeSpan.FromMinutes(1));
    }
}