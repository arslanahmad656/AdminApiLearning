using Aro.Admin.Application.Services;
using Aro.Admin.Infrastructure.Services;
using Aro.Admin.Tests.Common;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;

namespace Aro.Admin.Infrastructure.Services.Tests;

public class RequestInterpretorServiceTests : TestBase
{
    private readonly Mock<IHttpContextAccessor> mockHttpContextAccessor;
    private readonly Mock<ILogManager<RequestInterpretorService>> mockLogger;
    private readonly Mock<HttpContext> mockHttpContext;
    private readonly Mock<HttpRequest> mockRequest;
    private readonly Mock<ConnectionInfo> mockConnection;
    private readonly Mock<IServiceProvider> mockServiceProvider;
    private readonly RequestInterpretorService service;

    public RequestInterpretorServiceTests()
    {
        mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        mockLogger = new Mock<ILogManager<RequestInterpretorService>>();
        mockHttpContext = new Mock<HttpContext>();
        mockRequest = new Mock<HttpRequest>();
        mockConnection = new Mock<ConnectionInfo>();
        mockServiceProvider = new Mock<IServiceProvider>();

        mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(mockHttpContext.Object);
        mockHttpContext.Setup(x => x.Request).Returns(mockRequest.Object);
        mockHttpContext.Setup(x => x.Connection).Returns(mockConnection.Object);
        mockHttpContext.Setup(x => x.RequestServices).Returns(mockServiceProvider.Object);

        service = new RequestInterpretorService(mockHttpContextAccessor.Object, mockLogger.Object);
    }

    [Fact]
    public void ExtractUsername_WithAuthenticatedUser_ShouldReturnUsername()
    {
        var expectedUsername = fixture.Create<string>();
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Name, expectedUsername)
        }));

        mockHttpContext.Setup(x => x.User).Returns(claimsPrincipal);

        var result = service.ExtractUsername();

        result.Should().Be(expectedUsername);
    }

    [Fact]
    public void ExtractUsername_WithNullHttpContext_ShouldReturnNull()
    {
        mockHttpContextAccessor.Setup(x => x.HttpContext).Returns((HttpContext?)null);

        var result = service.ExtractUsername();

        result.Should().BeNull();
    }

    [Fact]
    public void ExtractUsername_WithNullUser_ShouldReturnNull()
    {
        mockHttpContext.Setup(x => x.User).Returns((ClaimsPrincipal?)null);

        var result = service.ExtractUsername();

        result.Should().BeNull();
    }

    [Fact]
    public void ExtractUsername_WithNullIdentity_ShouldReturnNull()
    {
        var claimsPrincipal = new ClaimsPrincipal();
        mockHttpContext.Setup(x => x.User).Returns(claimsPrincipal);

        var result = service.ExtractUsername();

        result.Should().BeNull();
    }

    [Fact]
    public void ExtractUsername_ShouldLogDebugMessages()
    {
        var expectedUsername = fixture.Create<string>();
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Name, expectedUsername)
        }));

        mockHttpContext.Setup(x => x.User).Returns(claimsPrincipal);

        service.ExtractUsername();

        mockLogger.Verify(x => x.LogDebug("Starting {MethodName}", "ExtractUsername"), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Username extracted: {Username}", expectedUsername), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Completed {MethodName}", "ExtractUsername"), Times.Once);
    }

    [Fact]
    public void ExtractUsername_WithEmptyUsername_ShouldLogEmptyString()
    {
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Name, string.Empty)
        }));

        mockHttpContext.Setup(x => x.User).Returns(claimsPrincipal);

        service.ExtractUsername();

        mockLogger.Verify(x => x.LogDebug("Username extracted: {Username}", string.Empty), Times.Once);
    }

    [Fact]
    public void RetrieveIpAddress_WithXForwardedForHeader_ShouldReturnFirstIp()
    {
        var forwardedIps = "192.168.1.1, 10.0.0.1, 172.16.0.1";
        var expectedIp = "192.168.1.1";
        var headers = new HeaderDictionary();
        headers.Add("X-Forwarded-For", forwardedIps);

        mockRequest.Setup(x => x.Headers).Returns(headers);

        var result = service.RetrieveIpAddress();

        result.Should().Be(expectedIp);
    }

    [Fact]
    public void RetrieveIpAddress_WithSingleXForwardedForHeader_ShouldReturnThatIp()
    {
        var expectedIp = fixture.Create<string>();
        var headers = new HeaderDictionary();
        headers.Add("X-Forwarded-For", expectedIp);

        mockRequest.Setup(x => x.Headers).Returns(headers);

        var result = service.RetrieveIpAddress();

        result.Should().Be(expectedIp);
    }

    [Fact]
    public void RetrieveIpAddress_WithEmptyXForwardedForHeader_ShouldFallbackToRemoteIp()
    {
        var expectedIp = "192.168.1.100";
        var headers = new HeaderDictionary();
        headers.Add("X-Forwarded-For", string.Empty);

        mockRequest.Setup(x => x.Headers).Returns(headers);
        mockConnection.Setup(x => x.RemoteIpAddress).Returns(System.Net.IPAddress.Parse(expectedIp));

        var result = service.RetrieveIpAddress();

        result.Should().Be(expectedIp);
    }

    [Fact]
    public void RetrieveIpAddress_WithNullXForwardedForHeader_ShouldFallbackToRemoteIp()
    {
        var expectedIp = "10.0.0.1";
        var headers = new HeaderDictionary();

        mockRequest.Setup(x => x.Headers).Returns(headers);
        mockConnection.Setup(x => x.RemoteIpAddress).Returns(System.Net.IPAddress.Parse(expectedIp));

        var result = service.RetrieveIpAddress();

        result.Should().Be(expectedIp);
    }

    [Fact]
    public void RetrieveIpAddress_WithNullHttpContext_ShouldReturnNull()
    {
        mockHttpContextAccessor.Setup(x => x.HttpContext).Returns((HttpContext?)null);

        var result = service.RetrieveIpAddress();

        result.Should().BeNull();
    }

    [Fact]
    public void RetrieveIpAddress_WithNullRequest_ShouldReturnNull()
    {
        mockHttpContext.Setup(x => x.Request).Returns((HttpRequest?)null);

        var result = service.RetrieveIpAddress();

        result.Should().BeNull();
    }

    [Fact]
    public void RetrieveIpAddress_WithNullConnection_ShouldReturnNull()
    {
        mockHttpContext.Setup(x => x.Connection).Returns((ConnectionInfo?)null);

        var result = service.RetrieveIpAddress();

        result.Should().BeNull();
    }

    [Fact]
    public void RetrieveIpAddress_ShouldLogDebugMessages()
    {
        var expectedIp = fixture.Create<string>();
        var headers = new HeaderDictionary();
        headers.Add("X-Forwarded-For", expectedIp);

        mockRequest.Setup(x => x.Headers).Returns(headers);

        service.RetrieveIpAddress();

        mockLogger.Verify(x => x.LogDebug("Starting {MethodName}", "RetrieveIpAddress"), Times.Once);
        mockLogger.Verify(x => x.LogDebug("X-Forwarded-For header: {ForwardedHeader}", expectedIp), Times.Once);
        mockLogger.Verify(x => x.LogDebug("IP address extracted from forwarded header: {IpAddress}", expectedIp), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Completed {MethodName}", "RetrieveIpAddress"), Times.Once);
    }

    [Fact]
    public void GetUserAgent_WithUserAgentHeader_ShouldReturnUserAgent()
    {
        var expectedUserAgent = fixture.Create<string>();
        var headers = new HeaderDictionary();
        headers.Add("User-Agent", expectedUserAgent);

        mockRequest.Setup(x => x.Headers).Returns(headers);

        var result = service.GetUserAgent();

        result.Should().Be(expectedUserAgent);
    }

    [Fact]
    public void GetUserAgent_WithEmptyUserAgentHeader_ShouldReturnEmptyString()
    {
        var headers = new HeaderDictionary();
        headers.Add("User-Agent", string.Empty);

        mockRequest.Setup(x => x.Headers).Returns(headers);

        var result = service.GetUserAgent();

        result.Should().Be(string.Empty);
    }

    [Fact]
    public void GetUserAgent_WithNullUserAgentHeader_ShouldReturnNull()
    {
        var headers = new HeaderDictionary();

        mockRequest.Setup(x => x.Headers).Returns(headers);

        var result = service.GetUserAgent();

        result.Should().BeNull();
    }

    [Fact]
    public void GetUserAgent_WithNullHttpContext_ShouldReturnNull()
    {
        mockHttpContextAccessor.Setup(x => x.HttpContext).Returns((HttpContext?)null);

        var result = service.GetUserAgent();

        result.Should().BeNull();
    }

    [Fact]
    public void GetUserAgent_WithNullRequest_ShouldReturnNull()
    {
        mockHttpContext.Setup(x => x.Request).Returns((HttpRequest?)null);

        var result = service.GetUserAgent();

        result.Should().BeNull();
    }

    [Fact]
    public void GetUserAgent_ShouldLogDebugMessages()
    {
        var expectedUserAgent = fixture.Create<string>();
        var headers = new HeaderDictionary();
        headers.Add("User-Agent", expectedUserAgent);

        mockRequest.Setup(x => x.Headers).Returns(headers);

        service.GetUserAgent();

        mockLogger.Verify(x => x.LogDebug("Starting {MethodName}", "GetUserAgent"), Times.Once);
        mockLogger.Verify(x => x.LogDebug("User-Agent header: {UserAgent}", expectedUserAgent), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Completed {MethodName}", "GetUserAgent"), Times.Once);
    }

    [Fact]
    public void GetUserAgent_WithEmptyUserAgent_ShouldLogEmptyString()
    {
        var headers = new HeaderDictionary();
        headers.Add("User-Agent", string.Empty);

        mockRequest.Setup(x => x.Headers).Returns(headers);

        service.GetUserAgent();

        mockLogger.Verify(x => x.LogDebug("User-Agent header: {UserAgent}", string.Empty), Times.Once);
    }

    [Fact]
    public void ExtractUsername_ShouldThrowWhenLoggerThrows()
    {
        mockLogger.Setup(x => x.LogDebug(It.IsAny<string>(), It.IsAny<object[]>()))
            .Throws(new InvalidOperationException("Logger failure"));

        Action act = () => service.ExtractUsername();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Logger failure");
    }

    [Fact]
    public void RetrieveIpAddress_ShouldThrowWhenLoggerThrows()
    {
        mockLogger.Setup(x => x.LogDebug(It.IsAny<string>(), It.IsAny<object[]>()))
            .Throws(new InvalidOperationException("Logger failure"));

        Action act = () => service.RetrieveIpAddress();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Logger failure");
    }

    [Fact]
    public void GetUserAgent_ShouldThrowWhenLoggerThrows()
    {
        mockLogger.Setup(x => x.LogDebug(It.IsAny<string>(), It.IsAny<object[]>()))
            .Throws(new InvalidOperationException("Logger failure"));

        Action act = () => service.GetUserAgent();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Logger failure");
    }
}
