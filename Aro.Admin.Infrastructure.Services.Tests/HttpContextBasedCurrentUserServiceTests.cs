using Aro.Admin.Application.Services;
using Aro.Admin.Application.Services.DTOs.ServiceResponses;
using Aro.Admin.Domain.Shared.Exceptions;
using Aro.Admin.Tests.Common;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Xunit;

namespace Aro.Admin.Infrastructure.Services.Tests;

public class HttpContextBasedCurrentUserServiceTests : TestBase
{
    private readonly Mock<IHttpContextAccessor> mockHttpContextAccessor;
    private readonly Mock<HttpContext> mockHttpContext;
    private readonly Mock<ClaimsPrincipal> mockUser;
    private readonly Mock<ClaimsIdentity> mockIdentity;
    private readonly ErrorCodes errorCodes;
    private readonly Mock<ILogManager<HttpContextBasedCurrentUserService>> mockLogger;
    private readonly HttpContextBasedCurrentUserService service;

    public HttpContextBasedCurrentUserServiceTests()
    {
        mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        mockHttpContext = new Mock<HttpContext>();
        mockUser = new Mock<ClaimsPrincipal>();
        mockIdentity = new Mock<ClaimsIdentity>();
        errorCodes = new ErrorCodes();
        mockLogger = new Mock<ILogManager<HttpContextBasedCurrentUserService>>();

        mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(mockHttpContext.Object);
        mockHttpContext.Setup(x => x.User).Returns(mockUser.Object);
        mockUser.Setup(x => x.Identity).Returns(mockIdentity.Object);

        service = new HttpContextBasedCurrentUserService(
            mockHttpContextAccessor.Object,
            errorCodes,
            mockLogger.Object
        );
    }

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateInstance()
    {
        var httpContextAccessor = new Mock<IHttpContextAccessor>();
        var errorCodes = new ErrorCodes();
        var logger = new Mock<ILogManager<HttpContextBasedCurrentUserService>>();

        var service = new HttpContextBasedCurrentUserService(
            httpContextAccessor.Object,
            errorCodes,
            logger.Object
        );

        service.Should().NotBeNull();
    }

    [Fact]
    public void GetCurrentUserId_WithValidUserIdClaim_ShouldReturnUserId()
    {
        var userId = Guid.NewGuid();
        var userIdClaim = new Claim(ClaimTypes.NameIdentifier, userId.ToString());
        var claims = new List<Claim> { userIdClaim };
        
        mockUser.Setup(x => x.FindFirst(ClaimTypes.NameIdentifier)).Returns(userIdClaim);

        var result = service.GetCurrentUserId();

        result.Should().Be(userId);
        mockLogger.Verify(x => x.LogDebug("Starting {MethodName}", "GetCurrentUserId"), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Current user ID retrieved: {UserId}", userId), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Completed {MethodName}", "GetCurrentUserId"), Times.Once);
    }

    [Fact]
    public void GetCurrentUserId_WithInvalidUserIdClaim_ShouldReturnNull()
    {
        var invalidUserIdClaim = new Claim(ClaimTypes.NameIdentifier, "invalid-guid");
        mockUser.Setup(x => x.FindFirst(ClaimTypes.NameIdentifier)).Returns(invalidUserIdClaim);

        var result = service.GetCurrentUserId();

        result.Should().BeNull();
        mockLogger.Verify(x => x.LogDebug("Starting {MethodName}", "GetCurrentUserId"), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Current user ID retrieved: {UserId}", default(Guid)), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Completed {MethodName}", "GetCurrentUserId"), Times.Once);
    }

    [Fact]
    public void GetCurrentUserId_WithNoUserIdClaim_ShouldReturnNull()
    {
        mockUser.Setup(x => x.FindFirst(ClaimTypes.NameIdentifier)).Returns((Claim?)null);

        var result = service.GetCurrentUserId();

        result.Should().BeNull();
        mockLogger.Verify(x => x.LogDebug("Starting {MethodName}", "GetCurrentUserId"), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Current user ID retrieved: {UserId}", default(Guid)), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Completed {MethodName}", "GetCurrentUserId"), Times.Once);
    }

    [Fact]
    public void GetCurrentUserId_WithNullHttpContext_ShouldReturnNull()
    {
        mockHttpContextAccessor.Setup(x => x.HttpContext).Returns((HttpContext?)null!);

        var result = service.GetCurrentUserId();

        result.Should().BeNull();
        mockLogger.Verify(x => x.LogDebug("Starting {MethodName}", "GetCurrentUserId"), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Current user ID retrieved: {UserId}", default(Guid)), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Completed {MethodName}", "GetCurrentUserId"), Times.Once);
    }

    [Fact]
    public void GetTokenInfo_WithValidClaims_ShouldReturnTokenInfo()
    {
        var jti = "test-jti-123";
        var expUnix = "1640995200"; // 2022-01-01 00:00:00 UTC
        var expectedExpiry = DateTimeOffset.FromUnixTimeSeconds(1640995200).UtcDateTime;

        var jtiClaim = new Claim(JwtRegisteredClaimNames.Jti, jti);
        var expClaim = new Claim(JwtRegisteredClaimNames.Exp, expUnix);
        var claims = new List<Claim> { jtiClaim, expClaim };

        mockUser.Setup(x => x.FindFirst(JwtRegisteredClaimNames.Jti)).Returns(jtiClaim);
        mockUser.Setup(x => x.FindFirst(JwtRegisteredClaimNames.Exp)).Returns(expClaim);

        var result = service.GetTokenInfo();

        result.Should().NotBeNull();
        result.TokenIdentifier.Should().Be(jti);
        result.Expiry.Should().Be(expectedExpiry);
        mockLogger.Verify(x => x.LogDebug("Starting {MethodName}", "GetTokenInfo"), Times.Once);
        mockLogger.Verify(x => x.LogDebug("User claims retrieved from HTTP context"), Times.Once);
        mockLogger.Verify(x => x.LogDebug("JWT claims extracted, jti: {Jti}, exp: {Exp}", jti, expUnix), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Token expiry calculated: {Expiry}", expectedExpiry), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Token info created successfully, jti: {Jti}, expiry: {Expiry}", jti, expectedExpiry), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Completed {MethodName}", "GetTokenInfo"), Times.Once);
    }

    [Fact]
    public void GetTokenInfo_WithMissingJtiClaim_ShouldThrowAroInvalidOperationException()
    {
        var expClaim = new Claim(JwtRegisteredClaimNames.Exp, "1640995200");
        mockUser.Setup(x => x.FindFirst(JwtRegisteredClaimNames.Jti)).Returns((Claim?)null);
        mockUser.Setup(x => x.FindFirst(JwtRegisteredClaimNames.Exp)).Returns(expClaim);

        Action act = () => service.GetTokenInfo();

        act.Should().Throw<AroInvalidOperationException>()
            .WithMessage($"Could not find {JwtRegisteredClaimNames.Jti} value in the claims.")
            .Which.ErrorCode.Should().Be(errorCodes.TOKEN_INFO_RETRIEVAL_ERROR);
    }

    [Fact]
    public void GetTokenInfo_WithMissingExpClaim_ShouldThrowAroInvalidOperationException()
    {
        var jtiClaim = new Claim(JwtRegisteredClaimNames.Jti, "test-jti-123");
        mockUser.Setup(x => x.FindFirst(JwtRegisteredClaimNames.Jti)).Returns(jtiClaim);
        mockUser.Setup(x => x.FindFirst(JwtRegisteredClaimNames.Exp)).Returns((Claim?)null);

        Action act = () => service.GetTokenInfo();

        act.Should().Throw<AroInvalidOperationException>()
            .WithMessage($"Could not find {JwtRegisteredClaimNames.Exp} value in the claims.")
            .Which.ErrorCode.Should().Be(errorCodes.TOKEN_INFO_RETRIEVAL_ERROR);
    }

    [Fact]
    public void GetTokenInfo_WithNullUser_ShouldThrowAroUnauthorizedException()
    {
        mockHttpContext.Setup(x => x.User).Returns((ClaimsPrincipal?)null!);

        Action act = () => service.GetTokenInfo();

        act.Should().Throw<AroUnauthorizedException>()
            .WithMessage("Cannot extract the token information out of an unauthorized context.")
            .Which.ErrorCode.Should().Be(errorCodes.TOKEN_INFO_RETRIEVAL_ERROR);
    }

    [Fact]
    public void GetTokenInfo_WithNullHttpContext_ShouldThrowAroInvalidOperationException()
    {
        mockHttpContextAccessor.Setup(x => x.HttpContext).Returns((HttpContext?)null!);

        Action act = () => service.GetTokenInfo();

        act.Should().Throw<AroInvalidOperationException>()
            .WithMessage($"Could not find {JwtRegisteredClaimNames.Jti} value in the claims.")
            .Which.ErrorCode.Should().Be(errorCodes.TOKEN_INFO_RETRIEVAL_ERROR);
    }

    [Fact]
    public void GetTokenInfo_WithInvalidExpClaim_ShouldThrowFormatException()
    {
        var jtiClaim = new Claim(JwtRegisteredClaimNames.Jti, "test-jti-123");
        var invalidExpClaim = new Claim(JwtRegisteredClaimNames.Exp, "invalid-unix-timestamp");
        
        mockUser.Setup(x => x.FindFirst(JwtRegisteredClaimNames.Jti)).Returns(jtiClaim);
        mockUser.Setup(x => x.FindFirst(JwtRegisteredClaimNames.Exp)).Returns(invalidExpClaim);

        Action act = () => service.GetTokenInfo();

        act.Should().Throw<FormatException>();
    }

    [Fact]
    public void IsAuthenticated_WithAuthenticatedUser_ShouldReturnTrue()
    {
        mockIdentity.Setup(x => x.IsAuthenticated).Returns(true);

        var result = service.IsAuthenticated();

        result.Should().BeTrue();
        mockLogger.Verify(x => x.LogDebug("Starting {MethodName}", "IsAuthenticated"), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Authentication status: {IsAuthenticated}", true), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Completed {MethodName}", "IsAuthenticated"), Times.Once);
    }

    [Fact]
    public void IsAuthenticated_WithUnauthenticatedUser_ShouldReturnFalse()
    {
        mockIdentity.Setup(x => x.IsAuthenticated).Returns(false);

        var result = service.IsAuthenticated();

        result.Should().BeFalse();
        mockLogger.Verify(x => x.LogDebug("Starting {MethodName}", "IsAuthenticated"), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Authentication status: {IsAuthenticated}", false), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Completed {MethodName}", "IsAuthenticated"), Times.Once);
    }

    [Fact]
    public void IsAuthenticated_WithNullIdentity_ShouldReturnFalse()
    {
        mockUser.Setup(x => x.Identity).Returns((ClaimsIdentity?)null);

        var result = service.IsAuthenticated();

        result.Should().BeFalse();
        mockLogger.Verify(x => x.LogDebug("Starting {MethodName}", "IsAuthenticated"), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Authentication status: {IsAuthenticated}", false), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Completed {MethodName}", "IsAuthenticated"), Times.Once);
    }

    [Fact]
    public void IsAuthenticated_WithNullUser_ShouldReturnFalse()
    {
        mockHttpContext.Setup(x => x.User).Returns((ClaimsPrincipal?)null!);

        var result = service.IsAuthenticated();

        result.Should().BeFalse();
        mockLogger.Verify(x => x.LogDebug("Starting {MethodName}", "IsAuthenticated"), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Authentication status: {IsAuthenticated}", false), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Completed {MethodName}", "IsAuthenticated"), Times.Once);
    }

    [Fact]
    public void IsAuthenticated_WithNullHttpContext_ShouldReturnFalse()
    {
        mockHttpContextAccessor.Setup(x => x.HttpContext).Returns((HttpContext?)null!);

        var result = service.IsAuthenticated();

        result.Should().BeFalse();
        mockLogger.Verify(x => x.LogDebug("Starting {MethodName}", "IsAuthenticated"), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Authentication status: {IsAuthenticated}", false), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Completed {MethodName}", "IsAuthenticated"), Times.Once);
    }

    [Fact]
    public void GetTokenInfo_WithDifferentExpiryTimes_ShouldCalculateCorrectly()
    {
        var jti = "test-jti-456";
        var expUnix = "1672531200"; // 2023-01-01 00:00:00 UTC
        var expectedExpiry = DateTimeOffset.FromUnixTimeSeconds(1672531200).UtcDateTime;

        var jtiClaim = new Claim(JwtRegisteredClaimNames.Jti, jti);
        var expClaim = new Claim(JwtRegisteredClaimNames.Exp, expUnix);

        mockUser.Setup(x => x.FindFirst(JwtRegisteredClaimNames.Jti)).Returns(jtiClaim);
        mockUser.Setup(x => x.FindFirst(JwtRegisteredClaimNames.Exp)).Returns(expClaim);

        var result = service.GetTokenInfo();

        result.Should().NotBeNull();
        result.TokenIdentifier.Should().Be(jti);
        result.Expiry.Should().Be(expectedExpiry);
    }

    [Fact]
    public void GetCurrentUserId_WithEmptyStringClaim_ShouldReturnNull()
    {
        var emptyUserIdClaim = new Claim(ClaimTypes.NameIdentifier, "");
        mockUser.Setup(x => x.FindFirst(ClaimTypes.NameIdentifier)).Returns(emptyUserIdClaim);

        var result = service.GetCurrentUserId();

        result.Should().BeNull();
    }

    [Fact]
    public void GetCurrentUserId_WithWhitespaceClaim_ShouldReturnNull()
    {
        var whitespaceUserIdClaim = new Claim(ClaimTypes.NameIdentifier, "   ");
        mockUser.Setup(x => x.FindFirst(ClaimTypes.NameIdentifier)).Returns(whitespaceUserIdClaim);

        var result = service.GetCurrentUserId();

        result.Should().BeNull();
    }

    [Fact]
    public void GetTokenInfo_WithZeroExpiry_ShouldHandleCorrectly()
    {
        var jti = "test-jti-zero";
        var expUnix = "0"; // Unix epoch
        var expectedExpiry = DateTimeOffset.FromUnixTimeSeconds(0).UtcDateTime;

        var jtiClaim = new Claim(JwtRegisteredClaimNames.Jti, jti);
        var expClaim = new Claim(JwtRegisteredClaimNames.Exp, expUnix);

        mockUser.Setup(x => x.FindFirst(JwtRegisteredClaimNames.Jti)).Returns(jtiClaim);
        mockUser.Setup(x => x.FindFirst(JwtRegisteredClaimNames.Exp)).Returns(expClaim);

        var result = service.GetTokenInfo();

        result.Should().NotBeNull();
        result.TokenIdentifier.Should().Be(jti);
        result.Expiry.Should().Be(expectedExpiry);
    }

    [Fact]
    public void GetTokenInfo_WithNegativeExpiry_ShouldHandleCorrectly()
    {
        var jti = "test-jti-negative";
        var expUnix = "-1"; // Before Unix epoch
        var expectedExpiry = DateTimeOffset.FromUnixTimeSeconds(-1).UtcDateTime;

        var jtiClaim = new Claim(JwtRegisteredClaimNames.Jti, jti);
        var expClaim = new Claim(JwtRegisteredClaimNames.Exp, expUnix);

        mockUser.Setup(x => x.FindFirst(JwtRegisteredClaimNames.Jti)).Returns(jtiClaim);
        mockUser.Setup(x => x.FindFirst(JwtRegisteredClaimNames.Exp)).Returns(expClaim);

        var result = service.GetTokenInfo();

        result.Should().NotBeNull();
        result.TokenIdentifier.Should().Be(jti);
        result.Expiry.Should().Be(expectedExpiry);
    }
}
