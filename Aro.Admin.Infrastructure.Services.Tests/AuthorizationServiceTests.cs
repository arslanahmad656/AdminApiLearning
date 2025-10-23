using Aro.Admin.Application.Services;
using Aro.Admin.Domain.Repository;
using Aro.Admin.Domain.Shared.Exceptions;
using Aro.Admin.Tests.Common;
using FluentAssertions;
using Moq;
using Xunit;

namespace Aro.Admin.Infrastructure.Services.Tests;

public class AuthorizationServiceTests : TestBase
{
    private readonly Mock<IRepositoryManager> mockRepositoryManager;
    private readonly Mock<IUserRepository> mockUserRepository;
    private readonly Mock<ICurrentUserService> mockCurrentUserService;
    private readonly Mock<ISystemContext> mockSystemContext;
    private readonly ErrorCodes errorCodes;
    private readonly Mock<ILogManager<AuthorizationService>> mockLogger;
    private readonly AuthorizationService service;

    public AuthorizationServiceTests()
    {
        mockRepositoryManager = new Mock<IRepositoryManager>();
        mockUserRepository = new Mock<IUserRepository>();
        mockCurrentUserService = new Mock<ICurrentUserService>();
        mockSystemContext = new Mock<ISystemContext>();
        errorCodes = new ErrorCodes();
        mockLogger = new Mock<ILogManager<AuthorizationService>>();

        mockRepositoryManager.Setup(x => x.UserRepository).Returns(mockUserRepository.Object);

        service = new AuthorizationService(
            mockRepositoryManager.Object,
            mockCurrentUserService.Object,
            mockSystemContext.Object,
            errorCodes,
            mockLogger.Object
        );
    }

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateInstance()
    {
        var repositoryManager = new Mock<IRepositoryManager>();
        var currentUserService = new Mock<ICurrentUserService>();
        var systemContext = new Mock<ISystemContext>();
        var errorCodes = new ErrorCodes();
        var logger = new Mock<ILogManager<AuthorizationService>>();

        var service = new AuthorizationService(
            repositoryManager.Object,
            currentUserService.Object,
            systemContext.Object,
            errorCodes,
            logger.Object
        );

        service.Should().NotBeNull();
    }

    [Fact]
    public async Task EnsureUserHasPermission_WithSystemContext_ShouldReturnImmediately()
    {
        var userId = Guid.NewGuid();
        var permissionCode = "test.permission";
        mockSystemContext.Setup(x => x.IsSystemContext).Returns(true);

        await service.EnsureUserHasPermission(userId, permissionCode, CancellationToken.None);

        mockLogger.Verify(x => x.LogDebug("Starting {MethodName}", "EnsureUserHasPermission"), Times.Once);
        mockUserRepository.Verify(x => x.GetById(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task UserHasPermission_WithSystemContext_ShouldReturnTrue()
    {
        var userId = Guid.NewGuid();
        var permissionCode = "test.permission";
        mockSystemContext.Setup(x => x.IsSystemContext).Returns(true);

        var result = await service.UserHasPermission(userId, permissionCode, CancellationToken.None);

        result.Should().BeTrue();
        mockLogger.Verify(x => x.LogDebug("Starting {MethodName}", "UserHasPermission"), Times.Once);
        mockUserRepository.Verify(x => x.GetById(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task CurrentUserHasPermissions_WithSystemContext_ShouldReturnTrue()
    {
        var permissions = new[] { "test.permission1", "test.permission2" };
        mockSystemContext.Setup(x => x.IsSystemContext).Returns(true);

        var result = await service.CurrentUserHasPermissions(permissions, CancellationToken.None);

        result.Should().BeTrue();
        mockLogger.Verify(x => x.LogDebug("Starting {MethodName}", "CurrentUserHasPermissions"), Times.Once);
        mockCurrentUserService.Verify(x => x.IsAuthenticated(), Times.Never);
    }

    [Fact]
    public async Task CurrentUserHasPermissions_WithUnauthenticatedUser_ShouldReturnFalse()
    {
        var permissions = new[] { "test.permission1", "test.permission2" };
        mockSystemContext.Setup(x => x.IsSystemContext).Returns(false);
        mockCurrentUserService.Setup(x => x.IsAuthenticated()).Returns(false);

        var result = await service.CurrentUserHasPermissions(permissions, CancellationToken.None);

        result.Should().BeFalse();
        mockLogger.Verify(x => x.LogDebug("Current user is not authenticated"), Times.Once);
    }

    [Fact]
    public async Task CurrentUserHasPermissions_WithNoUserId_ShouldReturnFalse()
    {
        var permissions = new[] { "test.permission1", "test.permission2" };
        mockSystemContext.Setup(x => x.IsSystemContext).Returns(false);
        mockCurrentUserService.Setup(x => x.IsAuthenticated()).Returns(true);
        mockCurrentUserService.Setup(x => x.GetCurrentUserId()).Returns((Guid?)null);

        var result = await service.CurrentUserHasPermissions(permissions, CancellationToken.None);

        result.Should().BeFalse();
        mockLogger.Verify(x => x.LogDebug("Current user ID is not available"), Times.Once);
    }

    [Fact]
    public async Task CurrentUserHasPermissions_WithEmptyPermissionsList_ShouldReturnTrue()
    {
        var permissions = new string[0];
        mockSystemContext.Setup(x => x.IsSystemContext).Returns(false);
        mockCurrentUserService.Setup(x => x.IsAuthenticated()).Returns(true);
        var userId = Guid.NewGuid();
        mockCurrentUserService.Setup(x => x.GetCurrentUserId()).Returns(userId);

        var result = await service.CurrentUserHasPermissions(permissions, CancellationToken.None);

        result.Should().BeTrue();
        mockLogger.Verify(x => x.LogDebug("Current user has all required permissions: {UserId}", userId), Times.Once);
    }

    [Fact]
    public async Task EnsureCurrentUserPermissions_WithSystemContext_ShouldReturnImmediately()
    {
        var permissions = new[] { "test.permission1", "test.permission2" };
        mockSystemContext.Setup(x => x.IsSystemContext).Returns(true);

        await service.EnsureCurrentUserPermissions(permissions, CancellationToken.None);

        mockLogger.Verify(x => x.LogDebug("Starting {MethodName}", "EnsureCurrentUserPermissions"), Times.Once);
        mockCurrentUserService.Verify(x => x.IsAuthenticated(), Times.Never);
    }

    [Fact]
    public async Task EnsureCurrentUserPermissions_WithUnauthenticatedUser_ShouldThrowAroException()
    {
        var permissions = new[] { "test.permission1", "test.permission2" };
        mockSystemContext.Setup(x => x.IsSystemContext).Returns(false);
        mockCurrentUserService.Setup(x => x.IsAuthenticated()).Returns(false);

        Func<Task> act = async () => await service.EnsureCurrentUserPermissions(permissions, CancellationToken.None);

        var exception = await act.Should().ThrowAsync<AroException>();
        exception.Which.ErrorCode.Should().Be(errorCodes.USER_NOT_AUTHENTICATED);

        mockLogger.Verify(x => x.LogWarn("Current user is not authenticated"), Times.Once);
    }

    [Fact]
    public async Task EnsureCurrentUserPermissions_WithNoUserId_ShouldThrowAroException()
    {
        var permissions = new[] { "test.permission1", "test.permission2" };
        mockSystemContext.Setup(x => x.IsSystemContext).Returns(false);
        mockCurrentUserService.Setup(x => x.IsAuthenticated()).Returns(true);
        mockCurrentUserService.Setup(x => x.GetCurrentUserId()).Returns((Guid?)null);

        Func<Task> act = async () => await service.EnsureCurrentUserPermissions(permissions, CancellationToken.None);

        var exception = await act.Should().ThrowAsync<AroException>();
        exception.Which.ErrorCode.Should().Be(errorCodes.USER_NOT_AUTHENTICATED);

        mockLogger.Verify(x => x.LogWarn("Current user ID is not available"), Times.Once);
    }

    [Fact]
    public async Task EnsureCurrentUserPermissions_WithEmptyPermissionsList_ShouldNotThrow()
    {
        var permissions = Array.Empty<string>();
        mockSystemContext.Setup(x => x.IsSystemContext).Returns(false);
        mockCurrentUserService.Setup(x => x.IsAuthenticated()).Returns(true);
        var userId = Guid.NewGuid();
        mockCurrentUserService.Setup(x => x.GetCurrentUserId()).Returns(userId);

        await service.EnsureCurrentUserPermissions(permissions, CancellationToken.None);

        mockLogger.Verify(x => x.LogInfo("Current user has all required permissions: {UserId}", userId), Times.Once);
    }

}