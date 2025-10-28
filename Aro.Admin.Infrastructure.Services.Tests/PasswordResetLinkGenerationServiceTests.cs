using Aro.Admin.Application.Services;
using Aro.Admin.Application.Services.DataServices;
using Aro.Admin.Application.Services.DTOs.ServiceParameters.PasswordLink;
using Aro.Admin.Application.Services.DTOs.ServiceParameters.PasswordReset;
using Aro.Admin.Application.Services.DTOs.ServiceResponses;
using Aro.Admin.Application.Shared.Options;
using Aro.Admin.Domain.Shared.Exceptions;
using Aro.Admin.Tests.Common;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Aro.Admin.Infrastructure.Services.Tests;

public class PasswordResetLinkGenerationServiceTests : TestBase
{
    private readonly Mock<IUserService> mockUserService;
    private readonly Mock<IRequestInterpretorService> mockRequestInterpretorService;
    private readonly Mock<IPasswordResetTokenService> mockPasswordResetTokenService;
    private readonly Mock<ILogManager<PasswordResetLinkGenerationService>> mockLogger;
    private readonly Mock<IOptionsSnapshot<PasswordResetSettings>> mockPasswordResetSettings;
    private readonly ErrorCodes errorCodes;
    private readonly PasswordResetLinkGenerationService service;

    public PasswordResetLinkGenerationServiceTests()
    {
        mockUserService = new Mock<IUserService>();
        mockRequestInterpretorService = new Mock<IRequestInterpretorService>();
        mockPasswordResetTokenService = new Mock<IPasswordResetTokenService>();
        mockLogger = new Mock<ILogManager<PasswordResetLinkGenerationService>>();
        errorCodes = new ErrorCodes();
        
        var passwordResetSettings = new PasswordResetSettings
        {
            TokenExpiryMinutes = 30,
            TokenLength = 32,
            EnforceSameIPandUserAgentForTokenUsage = true,
            FrontendResetPasswordUrl = "https://example.com/reset-password"
        };
        
        mockPasswordResetSettings = new Mock<IOptionsSnapshot<PasswordResetSettings>>();
        mockPasswordResetSettings.Setup(x => x.Value).Returns(passwordResetSettings);

        service = new PasswordResetLinkGenerationService(
            mockUserService.Object,
            mockRequestInterpretorService.Object,
            mockPasswordResetTokenService.Object,
            mockPasswordResetSettings.Object,
            errorCodes,
            mockLogger.Object
        );
    }

    [Fact]
    public async Task GenerateLink_WithValidParameters_ShouldReturnCorrectUri()
    {
        var email = "test@example.com";
        var userId = fixture.Create<Guid>();
        var ipAddress = "192.168.1.1";
        var userAgent = "Mozilla/5.0";
        var token = "test-token-123";
        var parameters = new GenerateLinkParameters(email);

        var userResponse = new GetUserResponse(
            userId,
            email,
            true,
            "Test User",
            "hashed-password",
            []
        );

        mockUserService.Setup(x => x.GetUserByEmail(email, false, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userResponse);
        mockRequestInterpretorService.Setup(x => x.RetrieveIpAddress()).Returns(ipAddress);
        mockRequestInterpretorService.Setup(x => x.GetUserAgent()).Returns(userAgent);
        mockPasswordResetTokenService.Setup(x => x.GenerateToken(
            It.Is<GenerateTokenParameters>(p => p.UserId == userId && p.RequestIp == ipAddress && p.UserAgent == userAgent),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(token);

        var result = await service.GenerateLink(parameters);

        result.Should().NotBeNull();
        result.Scheme.Should().Be("https");
        result.Host.Should().Be("example.com");
        result.AbsolutePath.Should().Be("/reset-password");
        result.Query.Should().Contain($"token={Uri.EscapeDataString(token)}");
        result.Query.Should().Contain($"email={Uri.EscapeDataString(email)}");

    }

    [Fact]
    public async Task GenerateLink_WithValidParametersAndCancellationToken_ShouldReturnCorrectUri()
    {
        var email = "test@example.com";
        var userId = fixture.Create<Guid>();
        var ipAddress = "192.168.1.1";
        var userAgent = "Mozilla/5.0";
        var token = "test-token-123";
        var parameters = new GenerateLinkParameters(email);
        var cancellationToken = new CancellationToken();

        var userResponse = new GetUserResponse(
            userId,
            email,
            true,
            "Test User",
            "hashed-password",
            []
        );

        mockUserService.Setup(x => x.GetUserByEmail(email, false, false, cancellationToken))
            .ReturnsAsync(userResponse);
        mockRequestInterpretorService.Setup(x => x.RetrieveIpAddress()).Returns(ipAddress);
        mockRequestInterpretorService.Setup(x => x.GetUserAgent()).Returns(userAgent);
        mockPasswordResetTokenService.Setup(x => x.GenerateToken(
            It.Is<GenerateTokenParameters>(p => p.UserId == userId && p.RequestIp == ipAddress && p.UserAgent == userAgent),
            cancellationToken))
            .ReturnsAsync(token);

        var result = await service.GenerateLink(parameters, cancellationToken);

        result.Should().NotBeNull();
        result.Scheme.Should().Be("https");
        result.Host.Should().Be("example.com");
        result.AbsolutePath.Should().Be("/reset-password");
        result.Query.Should().Contain($"token={Uri.EscapeDataString(token)}");
        result.Query.Should().Contain($"email={Uri.EscapeDataString(email)}");
    }

    [Fact]
    public async Task GenerateLink_WithSpecialCharactersInEmail_ShouldEscapeEmailCorrectly()
    {
        var email = "test+user@example.com";
        var userId = fixture.Create<Guid>();
        var ipAddress = "192.168.1.1";
        var userAgent = "Mozilla/5.0";
        var token = "test-token-123";
        var parameters = new GenerateLinkParameters(email);

        var userResponse = new GetUserResponse(
            userId,
            email,
            true,
            "Test User",
            "hashed-password",
            []
        );

        mockUserService.Setup(x => x.GetUserByEmail(email, false, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userResponse);
        mockRequestInterpretorService.Setup(x => x.RetrieveIpAddress()).Returns(ipAddress);
        mockRequestInterpretorService.Setup(x => x.GetUserAgent()).Returns(userAgent);
        mockPasswordResetTokenService.Setup(x => x.GenerateToken(
            It.IsAny<GenerateTokenParameters>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(token);

        var result = await service.GenerateLink(parameters);

        result.Should().NotBeNull();
        result.Query.Should().Contain($"email={Uri.EscapeDataString(email)}");
    }

    [Fact]
    public async Task GenerateLink_WithSpecialCharactersInToken_ShouldEscapeTokenCorrectly()
    {
        var email = "test@example.com";
        var userId = fixture.Create<Guid>();
        var ipAddress = "192.168.1.1";
        var userAgent = "Mozilla/5.0";
        var token = "test+token/with=special&chars?";
        var parameters = new GenerateLinkParameters(email);

        var userResponse = new GetUserResponse(
            userId,
            email,
            true,
            "Test User",
            "hashed-password",
            []
        );

        mockUserService.Setup(x => x.GetUserByEmail(email, false, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userResponse);
        mockRequestInterpretorService.Setup(x => x.RetrieveIpAddress()).Returns(ipAddress);
        mockRequestInterpretorService.Setup(x => x.GetUserAgent()).Returns(userAgent);
        mockPasswordResetTokenService.Setup(x => x.GenerateToken(
            It.IsAny<GenerateTokenParameters>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(token);

        var result = await service.GenerateLink(parameters);

        result.Should().NotBeNull();
        result.Query.Should().Contain($"token={Uri.EscapeDataString(token)}");
    }

    [Fact]
    public async Task GenerateLink_WithNullIpAddress_ShouldThrowAroEmailException()
    {
        var email = "test@example.com";
        var userId = fixture.Create<Guid>();
        var parameters = new GenerateLinkParameters(email);

        var userResponse = new GetUserResponse(
            userId,
            email,
            true,
            "Test User",
            "hashed-password",
            []
        );

        mockUserService.Setup(x => x.GetUserByEmail(email, false, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userResponse);
        mockRequestInterpretorService.Setup(x => x.RetrieveIpAddress()).Returns((string?)null);

        var action = async () => await service.GenerateLink(parameters);

        var exception = await action.Should().ThrowAsync<AroEmailException>();
        exception.Which.ErrorCode.Should().Be(errorCodes.EMAIL_LINK_GENERATION_ERROR);
        exception.Which.InnerException.Should().BeOfType<InvalidOperationException>();

    }

    [Fact]
    public async Task GenerateLink_WithNullUserAgent_ShouldThrowAroEmailException()
    {
        var email = "test@example.com";
        var userId = fixture.Create<Guid>();
        var ipAddress = "192.168.1.1";
        var parameters = new GenerateLinkParameters(email);

        var userResponse = new GetUserResponse(
            userId,
            email,
            true,
            "Test User",
            "hashed-password",
            []
        );

        mockUserService.Setup(x => x.GetUserByEmail(email, false, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userResponse);
        mockRequestInterpretorService.Setup(x => x.RetrieveIpAddress()).Returns(ipAddress);
        mockRequestInterpretorService.Setup(x => x.GetUserAgent()).Returns((string?)null);

        var action = async () => await service.GenerateLink(parameters);

        var exception = await action.Should().ThrowAsync<AroEmailException>();
        exception.Which.ErrorCode.Should().Be(errorCodes.EMAIL_LINK_GENERATION_ERROR);
        exception.Which.InnerException.Should().BeOfType<InvalidOperationException>();
    }

    [Fact]
    public async Task GenerateLink_WithUserServiceException_ShouldThrowAroEmailException()
    {
        var email = "test@example.com";
        var parameters = new GenerateLinkParameters(email);
        var expectedException = new InvalidOperationException("User not found");

        mockUserService.Setup(x => x.GetUserByEmail(email, false, false, It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

        var action = async () => await service.GenerateLink(parameters);

        var exception = await action.Should().ThrowAsync<AroEmailException>();
        exception.Which.ErrorCode.Should().Be(errorCodes.EMAIL_LINK_GENERATION_ERROR);
        exception.Which.InnerException.Should().BeOfType<InvalidOperationException>();
    }

    [Fact]
    public async Task GenerateLink_WithPasswordResetTokenServiceException_ShouldThrowAroEmailException()
    {
        var email = "test@example.com";
        var userId = fixture.Create<Guid>();
        var ipAddress = "192.168.1.1";
        var userAgent = "Mozilla/5.0";
        var parameters = new GenerateLinkParameters(email);
        var expectedException = new InvalidOperationException("Token generation failed");

        var userResponse = new GetUserResponse(
            userId,
            email,
            true,
            "Test User",
            "hashed-password",
            []
        );

        mockUserService.Setup(x => x.GetUserByEmail(email, false, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userResponse);
        mockRequestInterpretorService.Setup(x => x.RetrieveIpAddress()).Returns(ipAddress);
        mockRequestInterpretorService.Setup(x => x.GetUserAgent()).Returns(userAgent);
        mockPasswordResetTokenService.Setup(x => x.GenerateToken(
            It.IsAny<GenerateTokenParameters>(),
            It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

        var action = async () => await service.GenerateLink(parameters);

        var exception = await action.Should().ThrowAsync<AroEmailException>();
        exception.Which.ErrorCode.Should().Be(errorCodes.EMAIL_LINK_GENERATION_ERROR);
        exception.Which.InnerException.Should().BeOfType<InvalidOperationException>();
    }

    [Fact]
    public async Task GenerateLink_WithCancelledToken_ShouldThrowAroEmailException()
    {
        var email = "test@example.com";
        var userId = fixture.Create<Guid>();
        var ipAddress = "192.168.1.1";
        var userAgent = "Mozilla/5.0";
        var parameters = new GenerateLinkParameters(email);
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();
        var expectedException = new OperationCanceledException();

        var userResponse = new GetUserResponse(
            userId,
            email,
            true,
            "Test User",
            "hashed-password",
            []
        );

        mockUserService.Setup(x => x.GetUserByEmail(email, false, false, cancellationTokenSource.Token))
            .ReturnsAsync(userResponse);
        mockRequestInterpretorService.Setup(x => x.RetrieveIpAddress()).Returns(ipAddress);
        mockRequestInterpretorService.Setup(x => x.GetUserAgent()).Returns(userAgent);
        mockPasswordResetTokenService.Setup(x => x.GenerateToken(
            It.IsAny<GenerateTokenParameters>(),
            cancellationTokenSource.Token))
            .ThrowsAsync(expectedException);

        var action = async () => await service.GenerateLink(parameters, cancellationTokenSource.Token);

        var exception = await action.Should().ThrowAsync<AroEmailException>();
        exception.Which.ErrorCode.Should().Be(errorCodes.EMAIL_LINK_GENERATION_ERROR);
        exception.Which.InnerException.Should().BeOfType<OperationCanceledException>();
    }

    [Fact]
    public async Task GenerateLink_WithDifferentFrontendUrl_ShouldUseCorrectUrl()
    {
        var customSettings = new PasswordResetSettings
        {
            TokenExpiryMinutes = 30,
            TokenLength = 32,
            EnforceSameIPandUserAgentForTokenUsage = true,
            FrontendResetPasswordUrl = "https://custom.example.com/auth/reset"
        };

        var mockCustomSettings = new Mock<IOptionsSnapshot<PasswordResetSettings>>();
        mockCustomSettings.Setup(x => x.Value).Returns(customSettings);
        
        var customService = new PasswordResetLinkGenerationService(
            mockUserService.Object,
            mockRequestInterpretorService.Object,
            mockPasswordResetTokenService.Object,
            mockCustomSettings.Object,
            errorCodes,
            mockLogger.Object
        );

        var email = "test@example.com";
        var userId = fixture.Create<Guid>();
        var ipAddress = "192.168.1.1";
        var userAgent = "Mozilla/5.0";
        var token = "test-token-123";
        var parameters = new GenerateLinkParameters(email);

        var userResponse = new GetUserResponse(
            userId,
            email,
            true,
            "Test User",
            "hashed-password",
            []
        );

        mockUserService.Setup(x => x.GetUserByEmail(email, false, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userResponse);
        mockRequestInterpretorService.Setup(x => x.RetrieveIpAddress()).Returns(ipAddress);
        mockRequestInterpretorService.Setup(x => x.GetUserAgent()).Returns(userAgent);
        mockPasswordResetTokenService.Setup(x => x.GenerateToken(
            It.IsAny<GenerateTokenParameters>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(token);

        var result = await customService.GenerateLink(parameters);

        result.Should().NotBeNull();
        result.Scheme.Should().Be("https");
        result.Host.Should().Be("custom.example.com");
        result.AbsolutePath.Should().Be("/auth/reset");
        result.Query.Should().Contain($"token={Uri.EscapeDataString(token)}");
        result.Query.Should().Contain($"email={Uri.EscapeDataString(email)}");
    }

    [Fact]
    public async Task GenerateLink_WithHttpFrontendUrl_ShouldUseHttpScheme()
    {
        var httpSettings = new PasswordResetSettings
        {
            TokenExpiryMinutes = 30,
            TokenLength = 32,
            EnforceSameIPandUserAgentForTokenUsage = true,
            FrontendResetPasswordUrl = "http://localhost:3000/reset"
        };

        var mockHttpSettings = new Mock<IOptionsSnapshot<PasswordResetSettings>>();
        mockHttpSettings.Setup(x => x.Value).Returns(httpSettings);
        
        var httpService = new PasswordResetLinkGenerationService(
            mockUserService.Object,
            mockRequestInterpretorService.Object,
            mockPasswordResetTokenService.Object,
            mockHttpSettings.Object,
            errorCodes,
            mockLogger.Object
        );

        var email = "test@example.com";
        var userId = fixture.Create<Guid>();
        var ipAddress = "192.168.1.1";
        var userAgent = "Mozilla/5.0";
        var token = "test-token-123";
        var parameters = new GenerateLinkParameters(email);

        var userResponse = new GetUserResponse(
            userId,
            email,
            true,
            "Test User",
            "hashed-password",
            []
        );

        mockUserService.Setup(x => x.GetUserByEmail(email, false, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userResponse);
        mockRequestInterpretorService.Setup(x => x.RetrieveIpAddress()).Returns(ipAddress);
        mockRequestInterpretorService.Setup(x => x.GetUserAgent()).Returns(userAgent);
        mockPasswordResetTokenService.Setup(x => x.GenerateToken(
            It.IsAny<GenerateTokenParameters>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(token);

        var result = await httpService.GenerateLink(parameters);

        result.Should().NotBeNull();
        result.Scheme.Should().Be("http");
        result.Host.Should().Be("localhost");
        result.Port.Should().Be(3000);
        result.AbsolutePath.Should().Be("/reset");
    }

    [Fact]
    public async Task GenerateLink_WithEmptyToken_ShouldHandleEmptyToken()
    {
        var email = "test@example.com";
        var userId = fixture.Create<Guid>();
        var ipAddress = "192.168.1.1";
        var userAgent = "Mozilla/5.0";
        var token = "";
        var parameters = new GenerateLinkParameters(email);

        var userResponse = new GetUserResponse(
            userId,
            email,
            true,
            "Test User",
            "hashed-password",
            []
        );

        mockUserService.Setup(x => x.GetUserByEmail(email, false, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userResponse);
        mockRequestInterpretorService.Setup(x => x.RetrieveIpAddress()).Returns(ipAddress);
        mockRequestInterpretorService.Setup(x => x.GetUserAgent()).Returns(userAgent);
        mockPasswordResetTokenService.Setup(x => x.GenerateToken(
            It.IsAny<GenerateTokenParameters>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(token);

        var result = await service.GenerateLink(parameters);

        result.Should().NotBeNull();
        result.Query.Should().Contain($"token={Uri.EscapeDataString(token)}");
        result.Query.Should().Contain($"email={Uri.EscapeDataString(email)}");
    }

    [Fact]
    public async Task GenerateLink_WithLongToken_ShouldHandleLongToken()
    {
        var email = "test@example.com";
        var userId = fixture.Create<Guid>();
        var ipAddress = "192.168.1.1";
        var userAgent = "Mozilla/5.0";
        var token = new string('a', 1000);
        var parameters = new GenerateLinkParameters(email);

        var userResponse = new GetUserResponse(
            userId,
            email,
            true,
            "Test User",
            "hashed-password",
            []
        );

        mockUserService.Setup(x => x.GetUserByEmail(email, false, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userResponse);
        mockRequestInterpretorService.Setup(x => x.RetrieveIpAddress()).Returns(ipAddress);
        mockRequestInterpretorService.Setup(x => x.GetUserAgent()).Returns(userAgent);
        mockPasswordResetTokenService.Setup(x => x.GenerateToken(
            It.IsAny<GenerateTokenParameters>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(token);

        var result = await service.GenerateLink(parameters);

        result.Should().NotBeNull();
        result.Query.Should().Contain($"token={Uri.EscapeDataString(token)}");
        result.Query.Should().Contain($"email={Uri.EscapeDataString(email)}");
    }

    [Fact]
    public async Task GenerateLink_WithUnicodeEmail_ShouldEscapeUnicodeCorrectly()
    {
        var email = "tëst@éxämplé.com";
        var userId = fixture.Create<Guid>();
        var ipAddress = "192.168.1.1";
        var userAgent = "Mozilla/5.0";
        var token = "test-token-123";
        var parameters = new GenerateLinkParameters(email);

        var userResponse = new GetUserResponse(
            userId,
            email,
            true,
            "Test User",
            "hashed-password",
            []
        );

        mockUserService.Setup(x => x.GetUserByEmail(email, false, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userResponse);
        mockRequestInterpretorService.Setup(x => x.RetrieveIpAddress()).Returns(ipAddress);
        mockRequestInterpretorService.Setup(x => x.GetUserAgent()).Returns(userAgent);
        mockPasswordResetTokenService.Setup(x => x.GenerateToken(
            It.IsAny<GenerateTokenParameters>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(token);

        var result = await service.GenerateLink(parameters);

        result.Should().NotBeNull();
        result.Query.Should().Contain($"email={Uri.EscapeDataString(email)}");
    }
}
