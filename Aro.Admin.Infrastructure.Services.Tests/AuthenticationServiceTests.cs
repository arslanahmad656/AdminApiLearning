using Aro.Admin.Application.Services.AccessToken;
using Aro.Admin.Application.Services.DTOs.ServiceResponses;
using Aro.Admin.Application.Services.Hasher;
using Aro.Admin.Application.Services.Role;
using Aro.Admin.Application.Services.SystemContext;
using Aro.Admin.Application.Services.UniqueIdGenerator;
using Aro.Admin.Application.Services.User;
using Aro.Admin.Domain.Entities;
using Aro.Admin.Domain.Repository;
using Aro.Admin.Tests.Common;
using Aro.Common.Application.Services.LogManager;
using Aro.Common.Domain.Shared;
using Aro.Common.Domain.Shared.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;

namespace Aro.Admin.Infrastructure.Services.Tests;

public class AuthenticationServiceTests : TestBase
{
    private readonly Mock<IHasher> mockHasher;
    private readonly Mock<IUserService> mockUserService;
    private readonly Mock<IAccessTokenService> mockAccessTokenService;
    private readonly Mock<IRefreshTokenService> mockRefreshTokenService;
    private readonly Mock<IRepositoryManager> mockRepositoryManager;
    private readonly Mock<IRefreshTokenRepository> mockRefreshTokenRepository;
    private readonly Mock<IUniqueIdGenerator> mockIdGenerator;
    private readonly Mock<ITokenBlackListService> mockTokenBlackListService;
    private readonly Mock<IActiveAccessTokenService> mockActiveAccessTokenService;
    private readonly Mock<ILogManager<AuthenticationService>> mockLogger;
    private readonly Mock<ICurrentUserService> mockCurrentUserService;
    private readonly Mock<ISystemContextFactory> mockSystemContext;
    private readonly ErrorCodes errorCodes;
    private readonly AuthenticationService service;

    public AuthenticationServiceTests()
    {
        mockHasher = new Mock<IHasher>();
        mockUserService = new Mock<IUserService>();
        mockAccessTokenService = new Mock<IAccessTokenService>();
        mockRefreshTokenService = new Mock<IRefreshTokenService>();
        mockRepositoryManager = new Mock<IRepositoryManager>();
        mockRefreshTokenRepository = new Mock<IRefreshTokenRepository>();
        mockIdGenerator = new Mock<IUniqueIdGenerator>();
        mockTokenBlackListService = new Mock<ITokenBlackListService>();
        mockActiveAccessTokenService = new Mock<IActiveAccessTokenService>();
        mockLogger = new Mock<ILogManager<AuthenticationService>>();
        mockCurrentUserService = new Mock<ICurrentUserService>();
        mockSystemContext = new Mock<ISystemContextFactory>();
        errorCodes = new ErrorCodes();

        mockRepositoryManager.Setup(x => x.RefreshTokenRepository).Returns(mockRefreshTokenRepository.Object);

        service = new AuthenticationService(
            mockHasher.Object,
            mockUserService.Object,
            mockAccessTokenService.Object,
            mockRefreshTokenService.Object,
            mockRepositoryManager.Object,
            mockIdGenerator.Object,
            mockTokenBlackListService.Object,
            mockActiveAccessTokenService.Object,
            errorCodes,
            mockLogger.Object,
            mockCurrentUserService.Object,
            mockSystemContext.Object
        );
    }

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateInstance()
    {
        service.Should().NotBeNull();
    }

    [Fact]
    public async Task Authenticate_WithValidCredentials_ShouldReturnCompositeToken()
    {
        var email = "test@example.com";
        var password = "password123";
        var userId = Guid.NewGuid();
        var refreshTokenId = Guid.NewGuid();
        var accessToken = "access-token";
        var refreshToken = "refresh-token";
        var accessTokenExpiry = DateTime.UtcNow.AddHours(1);
        var refreshTokenExpiry = DateTime.UtcNow.AddDays(7);
        var accessTokenIdentifier = "token-identifier";

        var getUserResponse = new GetUserResponse(userId, email, true, "Test User", "hashed-password", new List<GetRoleRespose>());
        var accessTokenResponse = new AccessTokenResponse(accessToken, accessTokenExpiry, accessTokenIdentifier);
        var refreshTokenResponse = new Aro.Admin.Application.Services.DTOs.ServiceResponses.RefreshToken(refreshToken, refreshTokenExpiry);

        mockCurrentUserService.Setup(x => x.IsAuthenticated()).Returns(false);
        mockUserService.Setup(x => x.GetUserByEmail(email, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(getUserResponse);
        mockHasher.Setup(x => x.Verify(password, getUserResponse.PasswordHash)).Returns(true);
        mockAccessTokenService.Setup(x => x.GenerateAccessToken(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(accessTokenResponse);
        mockRefreshTokenService.Setup(x => x.GenerateRefreshToken(It.IsAny<CancellationToken>()))
            .ReturnsAsync(refreshTokenResponse);
        mockIdGenerator.Setup(x => x.Generate()).Returns(refreshTokenId);
        mockHasher.Setup(x => x.Hash(refreshToken)).Returns("hashed-refresh-token");

        var result = await service.Authenticate(email, password);

        result.Should().NotBeNull();
        result.UserId.Should().Be(userId);
        result.RefreshTokenId.Should().Be(refreshTokenId);
        result.AccessToken.Should().Be(accessToken);
        result.RefreshToken.Should().Be(refreshToken);
        result.AccessTokenExpiry.Should().Be(accessTokenExpiry);
        result.RefreshTokenExpiry.Should().Be(refreshTokenExpiry);
        result.AccessTokenIdentifier.Should().Be(accessTokenIdentifier);

        mockRefreshTokenRepository.Verify(x => x.Create(It.IsAny<Domain.Entities.RefreshToken>(), It.IsAny<CancellationToken>()), Times.Once);
        mockRepositoryManager.Verify(x => x.SaveChanges(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Authenticate_WithInvalidPassword_ShouldThrowAroException()
    {
        var email = "test@example.com";
        var password = "password123";
        var userId = Guid.NewGuid();

        var getUserResponse = new GetUserResponse(userId, email, true, "Test User", "hashed-password", new List<GetRoleRespose>());

        mockCurrentUserService.Setup(x => x.IsAuthenticated()).Returns(false);
        mockUserService.Setup(x => x.GetUserByEmail(email, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(getUserResponse);
        mockHasher.Setup(x => x.Verify(password, getUserResponse.PasswordHash)).Returns(false);

        var act = async () => await service.Authenticate(email, password);

        await act.Should().ThrowAsync<AroException>()
            .WithMessage($"Invalid password for user {email}.");

        mockLogger.Verify(x => x.LogWarn("Authentication failed for email: {Email} - invalid password", email), Times.Once);
    }

    [Fact]
    public async Task Authenticate_WhenUserIsAuthenticated_ShouldNotSetSystemContext()
    {
        var email = "test@example.com";
        var password = "password123";
        var userId = Guid.NewGuid();
        var refreshTokenId = Guid.NewGuid();

        var getUserResponse = new GetUserResponse(userId, email, true, "Test User", "hashed-password", new List<GetRoleRespose>());
        var accessTokenResponse = new AccessTokenResponse("access-token", DateTime.UtcNow.AddHours(1), "token-identifier");
        var refreshTokenResponse = new Aro.Admin.Application.Services.DTOs.ServiceResponses.RefreshToken("refresh-token", DateTime.UtcNow.AddDays(7));

        mockCurrentUserService.Setup(x => x.IsAuthenticated()).Returns(true);
        mockUserService.Setup(x => x.GetUserByEmail(email, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(getUserResponse);
        mockHasher.Setup(x => x.Verify(password, getUserResponse.PasswordHash)).Returns(true);
        mockAccessTokenService.Setup(x => x.GenerateAccessToken(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(accessTokenResponse);
        mockRefreshTokenService.Setup(x => x.GenerateRefreshToken(It.IsAny<CancellationToken>()))
            .ReturnsAsync(refreshTokenResponse);
        mockIdGenerator.Setup(x => x.Generate()).Returns(refreshTokenId);
        mockHasher.Setup(x => x.Hash(It.IsAny<string>())).Returns("hashed-refresh-token");

        await service.Authenticate(email, password);
    }

    [Fact]
    public async Task Authenticate_WithCancellationToken_ShouldPassTokenToServices()
    {
        var email = "test@example.com";
        var password = "password123";
        var userId = Guid.NewGuid();
        var refreshTokenId = Guid.NewGuid();
        var cancellationToken = new CancellationToken();

        var getUserResponse = new GetUserResponse(userId, email, true, "Test User", "hashed-password", new List<GetRoleRespose>());
        var accessTokenResponse = new AccessTokenResponse("access-token", DateTime.UtcNow.AddHours(1), "token-identifier");
        var refreshTokenResponse = new Aro.Admin.Application.Services.DTOs.ServiceResponses.RefreshToken("refresh-token", DateTime.UtcNow.AddDays(7));

        mockCurrentUserService.Setup(x => x.IsAuthenticated()).Returns(false);
        mockUserService.Setup(x => x.GetUserByEmail(email, false, true, cancellationToken))
            .ReturnsAsync(getUserResponse);
        mockHasher.Setup(x => x.Verify(password, getUserResponse.PasswordHash)).Returns(true);
        mockAccessTokenService.Setup(x => x.GenerateAccessToken(userId, cancellationToken))
            .ReturnsAsync(accessTokenResponse);
        mockRefreshTokenService.Setup(x => x.GenerateRefreshToken(cancellationToken))
            .ReturnsAsync(refreshTokenResponse);
        mockIdGenerator.Setup(x => x.Generate()).Returns(refreshTokenId);
        mockHasher.Setup(x => x.Hash(It.IsAny<string>())).Returns("hashed-refresh-token");

        await service.Authenticate(email, password, cancellationToken);

        mockUserService.Verify(x => x.GetUserByEmail(email, false, true, cancellationToken), Times.Once);
        mockAccessTokenService.Verify(x => x.GenerateAccessToken(userId, cancellationToken), Times.Once);
        mockRefreshTokenService.Verify(x => x.GenerateRefreshToken(cancellationToken), Times.Once);
        mockRefreshTokenRepository.Verify(x => x.Create(It.IsAny<Domain.Entities.RefreshToken>(), cancellationToken), Times.Once);
        mockRepositoryManager.Verify(x => x.SaveChanges(cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Logout_WithValidTokens_ShouldReturnTrue()
    {
        var userId = Guid.NewGuid();
        var refreshToken = "refresh-token";
        var accessTokenIdentifier = "token-identifier";
        var accessTokenExpiry = DateTime.UtcNow.AddHours(1);

        mockRefreshTokenService.Setup(x => x.Revoke(userId, refreshToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        mockTokenBlackListService.Setup(x => x.BlackList(accessTokenIdentifier, accessTokenExpiry, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await service.Logout(userId, refreshToken, accessTokenIdentifier, accessTokenExpiry);

        result.Should().BeTrue();
        mockRefreshTokenService.Verify(x => x.Revoke(userId, refreshToken, It.IsAny<CancellationToken>()), Times.Once);
        mockTokenBlackListService.Verify(x => x.BlackList(accessTokenIdentifier, accessTokenExpiry, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Logout_WithInvalidRefreshToken_ShouldReturnFalse()
    {
        var userId = Guid.NewGuid();
        var refreshToken = "refresh-token";
        var accessTokenIdentifier = "token-identifier";
        var accessTokenExpiry = DateTime.UtcNow.AddHours(1);

        mockRefreshTokenService.Setup(x => x.Revoke(userId, refreshToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        mockTokenBlackListService.Setup(x => x.BlackList(accessTokenIdentifier, accessTokenExpiry, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await service.Logout(userId, refreshToken, accessTokenIdentifier, accessTokenExpiry);

        result.Should().BeFalse();
        mockRefreshTokenService.Verify(x => x.Revoke(userId, refreshToken, It.IsAny<CancellationToken>()), Times.Once);
        mockTokenBlackListService.Verify(x => x.BlackList(accessTokenIdentifier, accessTokenExpiry, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Logout_WithCancellationToken_ShouldPassTokenToServices()
    {
        var userId = Guid.NewGuid();
        var refreshToken = "refresh-token";
        var accessTokenIdentifier = "token-identifier";
        var accessTokenExpiry = DateTime.UtcNow.AddHours(1);
        var cancellationToken = new CancellationToken();

        mockRefreshTokenService.Setup(x => x.Revoke(userId, refreshToken, cancellationToken))
            .ReturnsAsync(true);
        mockTokenBlackListService.Setup(x => x.BlackList(accessTokenIdentifier, accessTokenExpiry, cancellationToken))
            .Returns(Task.CompletedTask);

        await service.Logout(userId, refreshToken, accessTokenIdentifier, accessTokenExpiry, cancellationToken);

        mockRefreshTokenService.Verify(x => x.Revoke(userId, refreshToken, cancellationToken), Times.Once);
        mockTokenBlackListService.Verify(x => x.BlackList(accessTokenIdentifier, accessTokenExpiry, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task LogoutAll_WithValidUserId_ShouldRevokeAllTokens()
    {
        var userId = Guid.NewGuid();
        var activeTokens = new List<TokenInfo>
        {
            new TokenInfo("token-1", DateTime.UtcNow.AddHours(1)),
            new TokenInfo("token-2", DateTime.UtcNow.AddHours(2))
        };

        mockRefreshTokenService.Setup(x => x.RevokeAll(userId, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        mockActiveAccessTokenService.Setup(x => x.GetActiveTokens(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(activeTokens);
        mockTokenBlackListService.Setup(x => x.BlackList(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        mockActiveAccessTokenService.Setup(x => x.RemoveAllTokens(userId, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await service.LogoutAll(userId);

        mockRefreshTokenService.Verify(x => x.RevokeAll(userId, It.IsAny<CancellationToken>()), Times.Once);
        mockActiveAccessTokenService.Verify(x => x.GetActiveTokens(userId, It.IsAny<CancellationToken>()), Times.Once);
        mockTokenBlackListService.Verify(x => x.BlackList("token-1", activeTokens[0].Expiry, It.IsAny<CancellationToken>()), Times.Once);
        mockTokenBlackListService.Verify(x => x.BlackList("token-2", activeTokens[1].Expiry, It.IsAny<CancellationToken>()), Times.Once);
        mockActiveAccessTokenService.Verify(x => x.RemoveAllTokens(userId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task LogoutAll_WithNoActiveTokens_ShouldStillComplete()
    {
        var userId = Guid.NewGuid();
        var emptyTokens = new List<TokenInfo>();

        mockRefreshTokenService.Setup(x => x.RevokeAll(userId, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        mockActiveAccessTokenService.Setup(x => x.GetActiveTokens(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(emptyTokens);
        mockActiveAccessTokenService.Setup(x => x.RemoveAllTokens(userId, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await service.LogoutAll(userId);

        mockRefreshTokenService.Verify(x => x.RevokeAll(userId, It.IsAny<CancellationToken>()), Times.Once);
        mockActiveAccessTokenService.Verify(x => x.GetActiveTokens(userId, It.IsAny<CancellationToken>()), Times.Once);
        mockTokenBlackListService.Verify(x => x.BlackList(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()), Times.Never);
        mockActiveAccessTokenService.Verify(x => x.RemoveAllTokens(userId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task LogoutAll_WithCancellationToken_ShouldPassTokenToServices()
    {
        var userId = Guid.NewGuid();
        var cancellationToken = new CancellationToken();
        var activeTokens = new List<TokenInfo>
        {
            new TokenInfo("token-1", DateTime.UtcNow.AddHours(1))
        };

        mockRefreshTokenService.Setup(x => x.RevokeAll(userId, cancellationToken))
            .Returns(Task.CompletedTask);
        mockActiveAccessTokenService.Setup(x => x.GetActiveTokens(userId, cancellationToken))
            .ReturnsAsync(activeTokens);
        mockTokenBlackListService.Setup(x => x.BlackList(It.IsAny<string>(), It.IsAny<DateTime>(), cancellationToken))
            .Returns(Task.CompletedTask);
        mockActiveAccessTokenService.Setup(x => x.RemoveAllTokens(userId, cancellationToken))
            .Returns(Task.CompletedTask);

        await service.LogoutAll(userId, cancellationToken);

        mockRefreshTokenService.Verify(x => x.RevokeAll(userId, cancellationToken), Times.Once);
        mockActiveAccessTokenService.Verify(x => x.GetActiveTokens(userId, cancellationToken), Times.Once);
        mockTokenBlackListService.Verify(x => x.BlackList("token-1", activeTokens[0].Expiry, cancellationToken), Times.Once);
        mockActiveAccessTokenService.Verify(x => x.RemoveAllTokens(userId, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Authenticate_ShouldLogCorrectMessages()
    {
        var email = "test@example.com";
        var password = "password123";
        var userId = Guid.NewGuid();
        var refreshTokenId = Guid.NewGuid();

        var getUserResponse = new GetUserResponse(userId, email, true, "Test User", "hashed-password", new List<GetRoleRespose>());
        var accessTokenResponse = new AccessTokenResponse("access-token", DateTime.UtcNow.AddHours(1), "token-identifier");
        var refreshTokenResponse = new Aro.Admin.Application.Services.DTOs.ServiceResponses.RefreshToken("refresh-token", DateTime.UtcNow.AddDays(7));

        mockCurrentUserService.Setup(x => x.IsAuthenticated()).Returns(false);
        mockUserService.Setup(x => x.GetUserByEmail(email, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(getUserResponse);
        mockHasher.Setup(x => x.Verify(password, getUserResponse.PasswordHash)).Returns(true);
        mockAccessTokenService.Setup(x => x.GenerateAccessToken(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(accessTokenResponse);
        mockRefreshTokenService.Setup(x => x.GenerateRefreshToken(It.IsAny<CancellationToken>()))
            .ReturnsAsync(refreshTokenResponse);
        mockIdGenerator.Setup(x => x.Generate()).Returns(refreshTokenId);
        mockHasher.Setup(x => x.Hash(It.IsAny<string>())).Returns("hashed-refresh-token");

        await service.Authenticate(email, password);

        mockLogger.Verify(x => x.LogDebug("Starting {MethodName}", nameof(AuthenticationService.Authenticate)), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Retrieved user for authentication, userId: {UserId}, email: {Email}", userId, email), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Password verification completed for email: {Email}, result: {IsValid}", email, true), Times.Once);
        mockLogger.Verify(x => x.LogInfo("Generating access token for user: {UserId}", userId), Times.Once);
        mockLogger.Verify(x => x.LogInfo("Generating refresh token for user: {UserId}", userId), Times.Once);
        mockLogger.Verify(x => x.LogInfo("Authentication successful for email: {Email}, userId: {UserId}", email, userId), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Completed {MethodName}", nameof(AuthenticationService.Authenticate)), Times.Once);
    }

    [Fact]
    public async Task Logout_ShouldLogCorrectMessages()
    {
        var userId = Guid.NewGuid();
        var refreshToken = "refresh-token";
        var accessTokenIdentifier = "token-identifier";
        var accessTokenExpiry = DateTime.UtcNow.AddHours(1);

        mockRefreshTokenService.Setup(x => x.Revoke(userId, refreshToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        mockTokenBlackListService.Setup(x => x.BlackList(accessTokenIdentifier, accessTokenExpiry, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await service.Logout(userId, refreshToken, accessTokenIdentifier, accessTokenExpiry);

        mockLogger.Verify(x => x.LogDebug("Starting {MethodName}", nameof(AuthenticationService.Logout)), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Revoking refresh token for user: {UserId}", userId), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Refresh token revocation result for user: {UserId}, success: {Success}", userId, true), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Blacklisting access token: {AccessTokenIdentifier}, expiry: {Expiry}", accessTokenIdentifier, accessTokenExpiry), Times.Once);
        mockLogger.Verify(x => x.LogInfo("Logout completed for user: {UserId}, refreshTokenRevoked: {RefreshTokenRevoked}", userId, true), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Completed {MethodName}", nameof(AuthenticationService.Logout)), Times.Once);
    }

    [Fact]
    public async Task LogoutAll_ShouldLogCorrectMessages()
    {
        var userId = Guid.NewGuid();
        var activeTokens = new List<TokenInfo>
        {
            new TokenInfo("token-1", DateTime.UtcNow.AddHours(1))
        };

        mockRefreshTokenService.Setup(x => x.RevokeAll(userId, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        mockActiveAccessTokenService.Setup(x => x.GetActiveTokens(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(activeTokens);
        mockTokenBlackListService.Setup(x => x.BlackList(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        mockActiveAccessTokenService.Setup(x => x.RemoveAllTokens(userId, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await service.LogoutAll(userId);

        mockLogger.Verify(x => x.LogDebug("Starting {MethodName}", nameof(AuthenticationService.LogoutAll)), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Revoking all refresh tokens for user: {UserId}", userId), Times.Once);
        mockLogger.Verify(x => x.LogDebug("All refresh tokens revoked for user: {UserId}", userId), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Retrieving active access tokens for user: {UserId}", userId), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Found {TokenCount} active access tokens for user: {UserId}", 1, userId), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Blacklisting {TokenCount} active access tokens for user: {UserId}", 1, userId), Times.Once);
        mockLogger.Verify(x => x.LogDebug("All active access tokens blacklisted for user: {UserId}", userId), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Removing all tokens from active token service for user: {UserId}", userId), Times.Once);
        mockLogger.Verify(x => x.LogDebug("All tokens removed from active token service for user: {UserId}", userId), Times.Once);
        mockLogger.Verify(x => x.LogInfo("Logout all sessions completed for user: {UserId}, tokensProcessed: {TokenCount}", userId, 1), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Completed {MethodName}", nameof(AuthenticationService.LogoutAll)), Times.Once);
    }
}
