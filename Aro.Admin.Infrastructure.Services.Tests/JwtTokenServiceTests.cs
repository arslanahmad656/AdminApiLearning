using Aro.Admin.Application.Services;
using Aro.Admin.Application.Services.DataServices;
using Aro.Admin.Application.Services.DTOs.ServiceResponses;
using Aro.Admin.Application.Shared.Options;
using Aro.Admin.Domain.Shared;
using Aro.Admin.Tests.Common;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Xunit;

namespace Aro.Admin.Infrastructure.Services.Tests;

public class JwtTokenServiceTests : TestBase
{
    private readonly Mock<IUserService> mockUserService;
    private readonly Mock<IUniqueIdGenerator> mockIdGenerator;
    private readonly Mock<IActiveAccessTokenService> mockActiveAccessTokenService;
    private readonly Mock<IOptions<JwtOptions>> mockJwtOptions;
    private readonly Mock<ILogManager<JwtTokenService>> mockLogger;
    private readonly JwtOptions jwtOptions;
    private readonly SharedKeys sharedKeys;
    private readonly JwtTokenService service;

    public JwtTokenServiceTests()
    {
        mockUserService = new Mock<IUserService>();
        mockIdGenerator = new Mock<IUniqueIdGenerator>();
        mockActiveAccessTokenService = new Mock<IActiveAccessTokenService>();
        mockJwtOptions = new Mock<IOptions<JwtOptions>>();
        mockLogger = new Mock<ILogManager<JwtTokenService>>();

        jwtOptions = new JwtOptions
        {
            Issuer = "test-issuer",
            Audience = "test-audience",
            Key = "test-key-that-is-long-enough-for-hmac-sha256-algorithm",
            AccessTokenExpirationMinutes = 60,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true
        };

        sharedKeys = new SharedKeys();

        mockJwtOptions.Setup(x => x.Value).Returns(jwtOptions);

        service = new JwtTokenService(
            mockUserService.Object,
            mockIdGenerator.Object,
            mockActiveAccessTokenService.Object,
            mockJwtOptions.Object,
            sharedKeys,
            mockLogger.Object
        );
    }

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateInstance()
    {
        service.Should().NotBeNull();
    }

    [Fact]
    public async Task GenerateAccessToken_WithValidUserId_ShouldReturnAccessTokenResponse()
    {
        var userId = Guid.NewGuid();
        var jti = Guid.NewGuid();
        var user = new GetUserResponse(
            Id: userId,
            Email: "test@example.com",
            IsActive: true,
            DisplayName: "Test User",
            PasswordHash: "hashed-password",
            Roles: new List<GetRoleRespose>
            {
                new GetRoleRespose(Guid.NewGuid(), "Admin", "Administrator", true),
                new GetRoleRespose(Guid.NewGuid(), "User", "Regular User", false)
            }
        );

        mockUserService.Setup(x => x.GetUserById(userId, true, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        mockIdGenerator.Setup(x => x.Generate()).Returns(jti);

        var result = await service.GenerateAccessToken(userId);

        result.Should().NotBeNull();
        result.Token.Should().NotBeNullOrEmpty();
        result.Expiry.Should().BeCloseTo(DateTime.UtcNow.AddMinutes(jwtOptions.AccessTokenExpirationMinutes), TimeSpan.FromMinutes(1));
        result.TokenIdentifier.Should().Be(jti.ToString());

        mockUserService.Verify(x => x.GetUserById(userId, true, true, It.IsAny<CancellationToken>()), Times.Once);
        mockIdGenerator.Verify(x => x.Generate(), Times.Once);
        mockActiveAccessTokenService.Verify(x => x.RegisterToken(userId, jti.ToString(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GenerateAccessToken_WithValidUserId_ShouldCreateCorrectClaims()
    {
        var userId = Guid.NewGuid();
        var jti = Guid.NewGuid();
        var user = new GetUserResponse(
            Id: userId,
            Email: "test@example.com",
            IsActive: true,
            DisplayName: "Test User",
            PasswordHash: "hashed-password",
            Roles: new List<GetRoleRespose>
            {
                new GetRoleRespose(Guid.NewGuid(), "Admin", "Administrator", true)
            }
        );

        mockUserService.Setup(x => x.GetUserById(userId, true, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        mockIdGenerator.Setup(x => x.Generate()).Returns(jti);

        var result = await service.GenerateAccessToken(userId);

        result.Should().NotBeNull();
        result.Token.Should().NotBeNullOrEmpty();

        // Verify the token contains expected claims by decoding it
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(result.Token);

        token.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == userId.ToString());
        token.Claims.Should().Contain(c => c.Type == ClaimTypes.Name && c.Value == "Test User");
        token.Claims.Should().Contain(c => c.Type == ClaimTypes.Email && c.Value == "test@example.com");
        token.Claims.Should().Contain(c => c.Type == sharedKeys.JWT_CLAIM_ACTIVE && c.Value == "True");
        token.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Jti && c.Value == jti.ToString());
        token.Claims.Should().Contain(c => c.Type == ClaimTypes.Role);
    }

    [Fact]
    public async Task GenerateAccessToken_WithUserWithoutRoles_ShouldCreateTokenWithoutRoleClaims()
    {
        var userId = Guid.NewGuid();
        var jti = Guid.NewGuid();
        var user = new GetUserResponse(
            Id: userId,
            Email: "test@example.com",
            IsActive: true,
            DisplayName: "Test User",
            PasswordHash: "hashed-password",
            Roles: new List<GetRoleRespose>()
        );

        mockUserService.Setup(x => x.GetUserById(userId, true, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        mockIdGenerator.Setup(x => x.Generate()).Returns(jti);

        var result = await service.GenerateAccessToken(userId);

        result.Should().NotBeNull();
        result.Token.Should().NotBeNullOrEmpty();

        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(result.Token);

        token.Claims.Should().NotContain(c => c.Type == ClaimTypes.Role);
    }

    [Fact]
    public async Task GenerateAccessToken_WithInactiveUser_ShouldCreateTokenWithInactiveClaim()
    {
        var userId = Guid.NewGuid();
        var jti = Guid.NewGuid();
        var user = new GetUserResponse(
            Id: userId,
            Email: "test@example.com",
            IsActive: false,
            DisplayName: "Test User",
            PasswordHash: "hashed-password",
            Roles: new List<GetRoleRespose>()
        );

        mockUserService.Setup(x => x.GetUserById(userId, true, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        mockIdGenerator.Setup(x => x.Generate()).Returns(jti);

        var result = await service.GenerateAccessToken(userId);

        result.Should().NotBeNull();

        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(result.Token);

        token.Claims.Should().Contain(c => c.Type == sharedKeys.JWT_CLAIM_ACTIVE && c.Value == "False");
    }

    [Fact]
    public async Task GenerateAccessToken_WithCancellationToken_ShouldPassTokenToDependencies()
    {
        var userId = Guid.NewGuid();
        var jti = Guid.NewGuid();
        var cancellationToken = new CancellationToken();
        var user = new GetUserResponse(
            Id: userId,
            Email: "test@example.com",
            IsActive: true,
            DisplayName: "Test User",
            PasswordHash: "hashed-password",
            Roles: new List<GetRoleRespose>()
        );

        mockUserService.Setup(x => x.GetUserById(userId, true, true, cancellationToken))
            .ReturnsAsync(user);
        mockIdGenerator.Setup(x => x.Generate()).Returns(jti);

        var result = await service.GenerateAccessToken(userId, cancellationToken);

        result.Should().NotBeNull();
        mockUserService.Verify(x => x.GetUserById(userId, true, true, cancellationToken), Times.Once);
        mockActiveAccessTokenService.Verify(x => x.RegisterToken(userId, jti.ToString(), It.IsAny<DateTime>(), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task GenerateAccessToken_ShouldSetCorrectTokenExpiry()
    {
        var userId = Guid.NewGuid();
        var jti = Guid.NewGuid();
        var user = new GetUserResponse(
            Id: userId,
            Email: "test@example.com",
            IsActive: true,
            DisplayName: "Test User",
            PasswordHash: "hashed-password",
            Roles: new List<GetRoleRespose>()
        );

        mockUserService.Setup(x => x.GetUserById(userId, true, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        mockIdGenerator.Setup(x => x.Generate()).Returns(jti);

        var result = await service.GenerateAccessToken(userId);

        result.Should().NotBeNull();
        result.Expiry.Should().BeCloseTo(DateTime.UtcNow.AddMinutes(jwtOptions.AccessTokenExpirationMinutes), TimeSpan.FromMinutes(1));
    }

    [Fact]
    public async Task GenerateAccessToken_ShouldCreateTokenWithCorrectIssuerAndAudience()
    {
        var userId = Guid.NewGuid();
        var jti = Guid.NewGuid();
        var user = new GetUserResponse(
            Id: userId,
            Email: "test@example.com",
            IsActive: true,
            DisplayName: "Test User",
            PasswordHash: "hashed-password",
            Roles: new List<GetRoleRespose>()
        );

        mockUserService.Setup(x => x.GetUserById(userId, true, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        mockIdGenerator.Setup(x => x.Generate()).Returns(jti);

        var result = await service.GenerateAccessToken(userId);

        result.Should().NotBeNull();

        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(result.Token);

        token.Issuer.Should().Be(jwtOptions.Issuer);
        token.Audiences.Should().Contain(jwtOptions.Audience);
    }

    [Fact]
    public async Task GenerateAccessToken_ShouldRegisterTokenWithActiveAccessTokenService()
    {
        var userId = Guid.NewGuid();
        var jti = Guid.NewGuid();
        var user = new GetUserResponse(
            Id: userId,
            Email: "test@example.com",
            IsActive: true,
            DisplayName: "Test User",
            PasswordHash: "hashed-password",
            Roles: new List<GetRoleRespose>()
        );

        mockUserService.Setup(x => x.GetUserById(userId, true, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        mockIdGenerator.Setup(x => x.Generate()).Returns(jti);

        var result = await service.GenerateAccessToken(userId);

        result.Should().NotBeNull();

        mockActiveAccessTokenService.Verify(
            x => x.RegisterToken(
                userId, 
                jti.ToString(), 
                It.IsAny<DateTime>(), 
                It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Fact]
    public async Task GenerateAccessToken_WithMultipleRoles_ShouldIncludeAllRoleClaims()
    {
        var userId = Guid.NewGuid();
        var jti = Guid.NewGuid();
        var role1Id = Guid.NewGuid();
        var role2Id = Guid.NewGuid();
        var role3Id = Guid.NewGuid();
        
        var user = new GetUserResponse(
            Id: userId,
            Email: "test@example.com",
            IsActive: true,
            DisplayName: "Test User",
            PasswordHash: "hashed-password",
            Roles: new List<GetRoleRespose>
            {
                new GetRoleRespose(role1Id, "Admin", "Administrator", true),
                new GetRoleRespose(role2Id, "User", "Regular User", false),
                new GetRoleRespose(role3Id, "Manager", "Manager Role", false)
            }
        );

        mockUserService.Setup(x => x.GetUserById(userId, true, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        mockIdGenerator.Setup(x => x.Generate()).Returns(jti);

        var result = await service.GenerateAccessToken(userId);

        result.Should().NotBeNull();

        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(result.Token);

        var roleClaims = token.Claims.Where(c => c.Type == ClaimTypes.Role).ToList();
        roleClaims.Should().HaveCount(3);
        roleClaims.Should().Contain(c => c.Value == role1Id.ToString());
        roleClaims.Should().Contain(c => c.Value == role2Id.ToString());
        roleClaims.Should().Contain(c => c.Value == role3Id.ToString());
    }

    [Fact]
    public async Task GenerateAccessToken_ShouldLogAppropriateMessages()
    {
        var userId = Guid.NewGuid();
        var jti = Guid.NewGuid();
        var user = new GetUserResponse(
            Id: userId,
            Email: "test@example.com",
            IsActive: true,
            DisplayName: "Test User",
            PasswordHash: "hashed-password",
            Roles: new List<GetRoleRespose>()
        );

        mockUserService.Setup(x => x.GetUserById(userId, true, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        mockIdGenerator.Setup(x => x.Generate()).Returns(jti);

        var result = await service.GenerateAccessToken(userId);

        result.Should().NotBeNull();

        // Verify that logging methods were called (but don't verify specific log messages as requested)
        mockLogger.Verify(x => x.LogDebug(It.IsAny<string>(), It.IsAny<object[]>()), Times.AtLeastOnce);
        mockLogger.Verify(x => x.LogInfo(It.IsAny<string>(), It.IsAny<object[]>()), Times.AtLeastOnce);
    }
}
