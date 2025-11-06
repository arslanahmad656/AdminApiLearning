using System.Threading;
using Aro.Admin.Application.Services.Authorization;
using Aro.Admin.Application.Services.DTOs.ServiceParameters;
using Aro.Admin.Domain.Entities;
using Aro.Admin.Domain.Repository;
using Aro.Admin.Domain.Shared;
using Aro.Admin.Tests.Common;
using Aro.Common.Application.Services.LogManager;
using Aro.Common.Application.Services.UniqueIdGenerator;
using Aro.Common.Domain.Shared;
using Aro.Common.Domain.Shared.Exceptions;
using FluentAssertions;
using Moq;

namespace Aro.Admin.Infrastructure.Services.Tests;

public class GroupServiceTests : TestBase
{
    private readonly Mock<IRepositoryManager> mockRepositoryManager;

    private readonly Mock<IGroupRepository> mockGroupRepository;
    private readonly Mock<IUserRepository> mockUserRepository;
    private readonly Mock<IUniqueIdGenerator> mockIdGenerator;
    private readonly Mock<IAuthorizationService> mockAuthorizationService;
    private readonly Mock<ILogManager<GroupService>> mockLogger;
    private readonly ErrorCodes errorCodes;
    private readonly GroupService service;

    public GroupServiceTests()
    {
        mockRepositoryManager = new Mock<IRepositoryManager>();
        mockGroupRepository = new Mock<IGroupRepository>();
        mockUserRepository = new Mock<IUserRepository>();
        mockIdGenerator = new Mock<IUniqueIdGenerator>();
        mockAuthorizationService = new Mock<IAuthorizationService>();
        mockLogger = new Mock<ILogManager<GroupService>>();
        errorCodes = new ErrorCodes();

        mockRepositoryManager.Setup(x => x.GroupRepository).Returns(mockGroupRepository.Object);
        mockRepositoryManager.Setup(x => x.UserRepository).Returns(mockUserRepository.Object);

        service = new GroupService(
            mockRepositoryManager.Object,
            mockIdGenerator.Object,
            mockAuthorizationService.Object,
            mockLogger.Object,
            errorCodes
        );
    }

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateInstance()
    {
        var repositoryManager = new Mock<IRepositoryManager>();
        var idGenerator = new Mock<IUniqueIdGenerator>();
        var authorizationService = new Mock<IAuthorizationService>();
        var logger = new Mock<ILogManager<GroupService>>();
        var errorCodes = new ErrorCodes();

        var service = new GroupService(
            repositoryManager.Object,
            idGenerator.Object,
            authorizationService.Object,
            logger.Object,
            errorCodes
        );

        service.Should().NotBeNull();
    }

    #region CreateGroup()

    #region Positive Tests

    [Fact]
    public async Task CreateGroup_AuthorizationService_Passes()
    {
        // Arrange                                  
        var groupId = Guid.NewGuid();
        var primaryContactId = Guid.NewGuid();
        var groupDto = new CreateGroupDto(
            "Test Group",
            "123 Test St",
            null,
            "Test City",
            "12345",
            "Test Country",
            null,
            primaryContactId,
            true
        );

        Setup_EnsureCurrentUserHasPermissions(PermissionCodes.CreateGroup);

        //var users = new List<User> { new() { Id = primaryContactId } }.AsQueryable();
        //mockUserRepository
        //    .Setup(x => x.GetById(primaryContactId))
        //    .Returns(users);

        //mockIdGenerator.Setup(x => x.Generate())
        //    .Returns(groupId);

        //mockGroupRepository
        //    .Setup(x => x.Create(It.IsAny<Group>(), It.IsAny<CancellationToken>()))
        //    .Returns(Task.CompletedTask);

        //mockRepositoryManager
        //    .Setup(x => x.SaveChanges(It.IsAny<CancellationToken>()))
        //    .Returns(Task.CompletedTask);

        // Act
        try
        {
            await service.CreateGroup(groupDto, CancellationToken.None);
        }
        catch
        {

        }


        // Assert
        Verify_EnsureCurrentUserHasPermissions(PermissionCodes.CreateGroup);

        mockUserRepository.Verify(x =>
            x.GetById(primaryContactId),
            Times.Once);

        //mockIdGenerator.Verify(x => x.Generate(), Times.Once);

        //mockGroupRepository.Verify(x =>
        //    x.Create(It.IsAny<Group>(), It.IsAny<CancellationToken>()), Times.Once);

        //mockRepositoryManager.Verify(x =>
        //    x.SaveChanges(It.IsAny<CancellationToken>()),
        //    Times.Once);
    }

    #endregion

    #region Negative Tests

    [Fact]
    public async Task CreateGroup_AuthorizationService_Fails()
    {
        Setup_AuthorizationService_ThrowsException(PermissionCodes.CreateGroup);

        await Assert.ThrowsAsync<AroUnauthorizedException>(() =>
            service.CreateGroup(It.IsAny<CreateGroupDto>(), CancellationToken.None));

        mockUserRepository.Verify(x => x.GetById(It.IsAny<Guid>()), Times.Never);
        mockGroupRepository.Verify(x => x.Create(It.IsAny<Group>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion

    #endregion

    #region GetGroups()

    #region Positive Tests

    [Fact]
    public async Task GetGroups_AuthrozationServices_Passes()
    {
        // Arrange                                  
        var queryDto = new GetGroupsDto(
            String.Empty,
            1,
            5,
            "GroupName",
            true
        );

        Setup_EnsureCurrentUserHasPermissions(PermissionCodes.GetGroups);

        //mockGroupRepository
        //    .Setup(x => x.GetAll())
        //    .Returns(new List<Group>().AsQueryable());

        // Act
        try
        {
            await service.GetGroups(queryDto, CancellationToken.None);
        }
        catch
        {

        }

        // Assert
        Verify_EnsureCurrentUserHasPermissions(PermissionCodes.GetGroups);

    }

    #endregion

    #region Negative Tests

    [Fact]
    public async Task GetGroups_AuthorizationService_Fails()
    {
        Setup_AuthorizationService_ThrowsException(PermissionCodes.GetGroups);

        await Assert.ThrowsAsync<AroUnauthorizedException>(() =>
            service.GetGroups(It.IsAny<GetGroupsDto>(), CancellationToken.None));

        mockGroupRepository.Verify(x => x.GetAll(), Times.Never);
    }

    #endregion

    #endregion

    #region GetGroupById()

    #region Positive Tests

    [Fact]
    public async Task GetGroupById_AuthorizationService_Passes()
    {
        // Arrange
        var groupId = Guid.NewGuid();
        var queryDto = new GetGroupDto(
            groupId,
            String.Empty
        );

        Setup_EnsureCurrentUserHasPermissions(PermissionCodes.GetGroup);

        // Act
        try
        {
            await service.GetGroupById(queryDto, CancellationToken.None);
        }
        catch
        {

        }

        // Assert
        Verify_EnsureCurrentUserHasPermissions(PermissionCodes.GetGroup);

        Verify_Group_GetById_Called();
    }

    #endregion

    #region Negative Tests

    [Fact]
    public async Task GetGroup_AuthorizationService_Fails()
    {
        Setup_AuthorizationService_ThrowsException(PermissionCodes.GetGroup);

        await Assert.ThrowsAsync<AroUnauthorizedException>(() =>
            service.GetGroupById(It.IsAny<GetGroupDto>(), CancellationToken.None));

        mockGroupRepository.Verify(x => x.GetById(It.IsAny<Guid>()), Times.Never);
    }

    #endregion

    #endregion

    #region DeleteGroup()

    #region Positive Tests
    [Fact]
    public async Task DeleteGroup_AuthorizationService_Passes()
    {
        // Arrange
        var groupId = Guid.NewGuid();
        var queryDto = new DeleteGroupDto(
            groupId
        );

        Setup_EnsureCurrentUserHasPermissions(PermissionCodes.DeleteGroup);

        // Act
        try
        {
            await service.DeleteGroup(queryDto, CancellationToken.None);
        }
        catch
        {

        }

        // Assert
        Verify_EnsureCurrentUserHasPermissions(PermissionCodes.DeleteGroup);

        Verify_Group_GetById_Called();
    }
    #endregion

    #region Negative Tests

    [Fact]
    public async Task DeleteGroup_AuthorizationService_Fails()
    {
        Setup_AuthorizationService_ThrowsException(PermissionCodes.DeleteGroup);

        await Assert.ThrowsAsync<AroUnauthorizedException>(() =>
            service.DeleteGroup(It.IsAny<DeleteGroupDto>(), CancellationToken.None));

        mockGroupRepository.Verify(x => x.GetById(It.IsAny<Guid>()), Times.Never);
        mockGroupRepository.Verify(x => x.Delete(It.IsAny<Group>()), Times.Never);
        mockRepositoryManager.Verify(x => x.SaveChanges(It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion

    #endregion

    #region Utility methods

    private void Setup_EnsureCurrentUserHasPermissions(string permissionCode)
    {
        mockAuthorizationService
            .Setup(x => x.EnsureCurrentUserPermissions(
                It.Is<IEnumerable<string>>(p => p.Contains(permissionCode)),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
    }

    private void Setup_AuthorizationService_ThrowsException(string permissionCode)
    {
        mockAuthorizationService
            .Setup(x => x.EnsureCurrentUserPermissions(
                It.Is<IEnumerable<string>>(p => p.Contains(permissionCode)),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new AroUnauthorizedException("UNAUTHORIZED", "Insufficient permissions"));
    }

    private void Verify_EnsureCurrentUserHasPermissions(string permissionCode)
    {
        mockAuthorizationService.Verify(x =>
            x.EnsureCurrentUserPermissions(
                It.Is<IEnumerable<string>>(p => p.Contains(permissionCode)),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    private void Verify_Group_GetById_Called()
    {
        mockGroupRepository.Verify(x =>
            x.GetById(It.IsAny<Guid>()),
            Times.Once);
    }

    #endregion
}