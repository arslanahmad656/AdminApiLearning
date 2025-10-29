using Aro.Admin.Application.Services;
using Aro.Admin.Application.Services.DataServices;
using Aro.Admin.Application.Services.DTOs.ServiceResponses;
using Aro.Admin.Domain.Entities;
using Aro.Admin.Domain.Repository;
using Aro.Admin.Domain.Shared;
using Aro.Admin.Domain.Shared.Exceptions;
using Aro.Admin.Tests.Common;
using FluentAssertions;
using Moq;
using Xunit;

namespace Aro.Admin.Infrastructure.Services.Tests;

public class RoleServiceTests : TestBase
{
    private readonly Mock<IRepositoryManager> mockRepositoryManager;
    private readonly Mock<IUserRoleRepository> mockUserRoleRepository;
    private readonly Mock<IRoleRepository> mockRoleRepository;
    private readonly Mock<IAuthorizationService> mockAuthorizationService;
    private readonly Mock<ILogManager<RoleService>> mockLogger;
    private readonly RoleService service;

    public RoleServiceTests()
    {
        mockRepositoryManager = new Mock<IRepositoryManager>();
        mockUserRoleRepository = new Mock<IUserRoleRepository>();
        mockRoleRepository = new Mock<IRoleRepository>();
        mockAuthorizationService = new Mock<IAuthorizationService>();
        mockLogger = new Mock<ILogManager<RoleService>>();

        mockRepositoryManager.Setup(x => x.UserRoleRepository).Returns(mockUserRoleRepository.Object);
        mockRepositoryManager.Setup(x => x.RoleRepository).Returns(mockRoleRepository.Object);

        service = new RoleService(
            mockRepositoryManager.Object,
            mockAuthorizationService.Object,
            mockLogger.Object
        );
    }

    [Fact]
    public async Task AssignRolesToUsers_WithValidInputs_ShouldAssignRolesSuccessfully()
    {
        var userIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        var roleIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        var cancellationToken = new CancellationToken();

        mockAuthorizationService.Setup(x => x.EnsureCurrentUserPermissions(
            It.IsAny<IEnumerable<string>>(), 
            It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        mockUserRoleRepository.Setup(x => x.Create(It.IsAny<UserRole>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        mockRepositoryManager.Setup(x => x.SaveChanges(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await service.AssignRolesToUsers(userIds, roleIds, cancellationToken);

        mockAuthorizationService.Verify(x => x.EnsureCurrentUserPermissions(
            new[] { PermissionCodes.AssignUserRole }, 
            cancellationToken), Times.Once);

        mockUserRoleRepository.Verify(x => x.Create(It.IsAny<UserRole>(), cancellationToken), 
            Times.Exactly(4));

        mockRepositoryManager.Verify(x => x.SaveChanges(cancellationToken), Times.Once);

        mockLogger.Verify(x => x.LogDebug("Starting {MethodName}", "AssignRolesToUsers"), Times.Once);
        mockLogger.Verify(x => x.LogInfo("Successfully assigned roles to users, assignmentCount: {AssignmentCount}", 4), Times.Once);
    }

    [Fact]
    public async Task AssignRolesToUsers_WithEmptyUserIds_ShouldNotCreateAnyAssignments()
    {
        var userIds = new List<Guid>();
        var roleIds = new List<Guid> { Guid.NewGuid() };
        var cancellationToken = new CancellationToken();

        mockAuthorizationService.Setup(x => x.EnsureCurrentUserPermissions(
            It.IsAny<IEnumerable<string>>(), 
            It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await service.AssignRolesToUsers(userIds, roleIds, cancellationToken);

        mockUserRoleRepository.Verify(x => x.Create(It.IsAny<UserRole>(), It.IsAny<CancellationToken>()), 
            Times.Never);
    }

    [Fact]
    public async Task AssignRolesToUsers_WithEmptyRoleIds_ShouldNotCreateAnyAssignments()
    {
        var userIds = new List<Guid> { Guid.NewGuid() };
        var roleIds = new List<Guid>();
        var cancellationToken = new CancellationToken();

        mockAuthorizationService.Setup(x => x.EnsureCurrentUserPermissions(
            It.IsAny<IEnumerable<string>>(), 
            It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await service.AssignRolesToUsers(userIds, roleIds, cancellationToken);

        mockUserRoleRepository.Verify(x => x.Create(It.IsAny<UserRole>(), It.IsAny<CancellationToken>()), 
            Times.Never);
    }

    [Fact]
    public async Task UserHasRole_WithValidUserAndRoleId_ShouldReturnTrue()
    {
        var userId = Guid.NewGuid();
        var roleId = Guid.NewGuid();
        var cancellationToken = new CancellationToken();

        mockAuthorizationService.Setup(x => x.EnsureCurrentUserPermissions(
            It.IsAny<IEnumerable<string>>(), 
            It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        mockUserRoleRepository.Setup(x => x.Exists(userId, roleId, cancellationToken))
            .ReturnsAsync(true);

        var result = await service.UserHasRole(userId, roleId, cancellationToken);

        result.Should().BeTrue();

        mockAuthorizationService.Verify(x => x.EnsureCurrentUserPermissions(
            new[] { PermissionCodes.TestUserRole }, 
            cancellationToken), Times.Once);

        mockUserRoleRepository.Verify(x => x.Exists(userId, roleId, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task UserHasRole_WithNonExistentRoleId_ShouldReturnFalse()
    {
        var userId = Guid.NewGuid();
        var roleId = Guid.NewGuid();
        var cancellationToken = new CancellationToken();

        mockAuthorizationService.Setup(x => x.EnsureCurrentUserPermissions(
            It.IsAny<IEnumerable<string>>(), 
            It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        mockUserRoleRepository.Setup(x => x.Exists(userId, roleId, cancellationToken))
            .ReturnsAsync(false);

        var result = await service.UserHasRole(userId, roleId, cancellationToken);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task AssignRolesToUsers_WithUnauthorizedUser_ShouldThrowException()
    {
        var userIds = new List<Guid> { Guid.NewGuid() };
        var roleIds = new List<Guid> { Guid.NewGuid() };
        var cancellationToken = new CancellationToken();

        mockAuthorizationService.Setup(x => x.EnsureCurrentUserPermissions(
            It.IsAny<IEnumerable<string>>(), 
            It.IsAny<CancellationToken>()))
            .ThrowsAsync(new AroUnauthorizedException("UNAUTHORIZED", "Insufficient permissions"));

        await Assert.ThrowsAsync<AroUnauthorizedException>(() => 
            service.AssignRolesToUsers(userIds, roleIds, cancellationToken));
    }
}