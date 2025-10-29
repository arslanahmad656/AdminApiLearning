using Aro.Admin.Application.Services;
using Aro.Admin.Application.Services.DataServices;
using Aro.Admin.Domain.Entities;
using Aro.Admin.Domain.Repository;
using Aro.Admin.Domain.Shared;
using Aro.Admin.Domain.Shared.Exceptions;
using Aro.Admin.Tests.Common;
using FluentAssertions;
using Moq;
using Xunit;

namespace Aro.Admin.Infrastructure.Services.Tests;

public class SystemSettingsServiceTests : TestBase
{
    private readonly Mock<IRepositoryManager> mockRepositoryManager;
    private readonly Mock<ISystemSettingsRepository> mockSystemSettingsRepository;
    private readonly Mock<IUserRepository> mockUserRepository;
    private readonly Mock<IAuthorizationService> mockAuthorizationService;
    private readonly Mock<ILogManager<SystemSettingsService>> mockLogger;
    private readonly SharedKeys sharedKeys;
    private readonly SystemSettingsService service;

    public SystemSettingsServiceTests()
    {
        mockRepositoryManager = new Mock<IRepositoryManager>();
        mockSystemSettingsRepository = new Mock<ISystemSettingsRepository>();
        mockUserRepository = new Mock<IUserRepository>();
        mockAuthorizationService = new Mock<IAuthorizationService>();
        mockLogger = new Mock<ILogManager<SystemSettingsService>>();
        sharedKeys = new SharedKeys();

        mockRepositoryManager.Setup(x => x.SystemSettingsRepository).Returns(mockSystemSettingsRepository.Object);
        mockRepositoryManager.Setup(x => x.UserRepository).Returns(mockUserRepository.Object);

        service = new SystemSettingsService(
            mockRepositoryManager.Object,
            sharedKeys,
            mockAuthorizationService.Object,
            mockLogger.Object
        );
    }

    [Fact]
    public async Task IsMigrationComplete_WithTrueValue_ShouldReturnTrue()
    {
        var cancellationToken = new CancellationToken();
        var setting = new SystemSetting { Key = sharedKeys.IS_MIGRATIONS_COMPLETE, Value = "True" };

        mockAuthorizationService.Setup(x => x.EnsureCurrentUserPermissions(
            It.IsAny<IEnumerable<string>>(), 
            It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        mockSystemSettingsRepository.Setup(x => x.GetValue(sharedKeys.IS_MIGRATIONS_COMPLETE, cancellationToken))
            .ReturnsAsync(setting);

        var result = await service.IsMigrationComplete(cancellationToken);

        result.Should().BeTrue();

        mockAuthorizationService.Verify(x => x.EnsureCurrentUserPermissions(
            new[] { PermissionCodes.GetSystemSettings }, 
            cancellationToken), Times.Once);

        mockLogger.Verify(x => x.LogDebug("Starting {MethodName}", "IsMigrationComplete"), Times.Once);
    }

    [Fact]
    public async Task IsMigrationComplete_WithFalseValue_ShouldReturnFalse()
    {
        var cancellationToken = new CancellationToken();
        var setting = new SystemSetting { Key = sharedKeys.IS_MIGRATIONS_COMPLETE, Value = "False" };

        mockAuthorizationService.Setup(x => x.EnsureCurrentUserPermissions(
            It.IsAny<IEnumerable<string>>(), 
            It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        mockSystemSettingsRepository.Setup(x => x.GetValue(sharedKeys.IS_MIGRATIONS_COMPLETE, cancellationToken))
            .ReturnsAsync(setting);

        var result = await service.IsMigrationComplete(cancellationToken);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsMigrationComplete_WithNullSetting_ShouldReturnFalse()
    {
        var cancellationToken = new CancellationToken();

        mockAuthorizationService.Setup(x => x.EnsureCurrentUserPermissions(
            It.IsAny<IEnumerable<string>>(), 
            It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        mockSystemSettingsRepository.Setup(x => x.GetValue(sharedKeys.IS_MIGRATIONS_COMPLETE, cancellationToken))
            .ReturnsAsync((SystemSetting?)null);

        var result = await service.IsMigrationComplete(cancellationToken);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task SetMigrationStateToComplete_WithValidInput_ShouldUpdateSetting()
    {
        var cancellationToken = new CancellationToken();
        var existingSetting = new SystemSetting { Key = sharedKeys.IS_MIGRATIONS_COMPLETE, Value = "False" };

        mockAuthorizationService.Setup(x => x.EnsureCurrentUserPermissions(
            It.IsAny<IEnumerable<string>>(), 
            It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        mockSystemSettingsRepository.Setup(x => x.GetValue(sharedKeys.IS_MIGRATIONS_COMPLETE, cancellationToken))
            .ReturnsAsync(existingSetting);

        mockSystemSettingsRepository.Setup(x => x.UpdateSetting(It.IsAny<SystemSetting>()));

        mockRepositoryManager.Setup(x => x.SaveChanges(cancellationToken))
            .Returns(Task.CompletedTask);

        await service.SetMigrationStateToComplete(cancellationToken);

        mockAuthorizationService.Verify(x => x.EnsureCurrentUserPermissions(
            new[] { PermissionCodes.MigrateDabase }, 
            cancellationToken), Times.Once);

        mockSystemSettingsRepository.Verify(x => x.UpdateSetting(It.Is<SystemSetting>(s => s.Value == "True")), Times.Once);
        mockRepositoryManager.Verify(x => x.SaveChanges(cancellationToken), Times.Once);

        mockLogger.Verify(x => x.LogDebug("Starting {MethodName}", "SetMigrationStateToComplete"), Times.Once);
        mockLogger.Verify(x => x.LogInfo("migration state set to completed successfully"), Times.Once);
    }

    [Fact]
    public async Task SetMigrationStateToComplete_WithNullSetting_ShouldCreateNewSetting()
    {
        var cancellationToken = new CancellationToken();

        mockAuthorizationService.Setup(x => x.EnsureCurrentUserPermissions(
            It.IsAny<IEnumerable<string>>(), 
            It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        mockSystemSettingsRepository.Setup(x => x.GetValue(sharedKeys.IS_MIGRATIONS_COMPLETE, cancellationToken))
            .ReturnsAsync((SystemSetting?)null);

        mockSystemSettingsRepository.Setup(x => x.UpdateSetting(It.IsAny<SystemSetting>()));

        mockRepositoryManager.Setup(x => x.SaveChanges(cancellationToken))
            .Returns(Task.CompletedTask);

        await service.SetMigrationStateToComplete(cancellationToken);

        mockSystemSettingsRepository.Verify(x => x.UpdateSetting(It.Is<SystemSetting>(s => 
            s.Key == sharedKeys.IS_MIGRATIONS_COMPLETE && s.Value == "True")), Times.Once);
    }

    [Fact]
    public async Task IsSystemInitialized_WithTrueSetting_ShouldReturnTrue()
    {
        var cancellationToken = new CancellationToken();
        var setting = new SystemSetting { Key = sharedKeys.IS_SYSTEM_INITIALIZED, Value = "True" };

        mockAuthorizationService.Setup(x => x.EnsureCurrentUserPermissions(
            It.IsAny<IEnumerable<string>>(), 
            It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        mockSystemSettingsRepository.Setup(x => x.GetValue(sharedKeys.IS_SYSTEM_INITIALIZED, cancellationToken))
            .ReturnsAsync(setting);

        var result = await service.IsSystemInitialized(cancellationToken);

        result.Should().BeTrue();

        mockAuthorizationService.Verify(x => x.EnsureCurrentUserPermissions(
            new[] { PermissionCodes.GetSystemSettings }, 
            cancellationToken), Times.Once);

        mockUserRepository.Verify(x => x.UsersExist(cancellationToken), Times.Never);
    }

    [Fact]
    public async Task IsSystemInitialized_WithFalseSettingAndUsersExist_ShouldReturnTrue()
    {
        var cancellationToken = new CancellationToken();
        var setting = new SystemSetting { Key = sharedKeys.IS_SYSTEM_INITIALIZED, Value = "False" };

        mockAuthorizationService.Setup(x => x.EnsureCurrentUserPermissions(
            It.IsAny<IEnumerable<string>>(), 
            It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        mockSystemSettingsRepository.Setup(x => x.GetValue(sharedKeys.IS_SYSTEM_INITIALIZED, cancellationToken))
            .ReturnsAsync(setting);

        mockUserRepository.Setup(x => x.UsersExist(cancellationToken))
            .ReturnsAsync(true);

        var result = await service.IsSystemInitialized(cancellationToken);

        result.Should().BeTrue();

        mockUserRepository.Verify(x => x.UsersExist(cancellationToken), Times.Once);
    }

    [Fact]
    public async Task IsSystemInitialized_WithFalseSettingAndNoUsers_ShouldReturnFalse()
    {
        var cancellationToken = new CancellationToken();
        var setting = new SystemSetting { Key = sharedKeys.IS_SYSTEM_INITIALIZED, Value = "False" };

        mockAuthorizationService.Setup(x => x.EnsureCurrentUserPermissions(
            It.IsAny<IEnumerable<string>>(), 
            It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        mockSystemSettingsRepository.Setup(x => x.GetValue(sharedKeys.IS_SYSTEM_INITIALIZED, cancellationToken))
            .ReturnsAsync(setting);

        mockUserRepository.Setup(x => x.UsersExist(cancellationToken))
            .ReturnsAsync(false);

        var result = await service.IsSystemInitialized(cancellationToken);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsSystemInitialized_WithNullSettingAndUsersExist_ShouldReturnTrue()
    {
        var cancellationToken = new CancellationToken();

        mockAuthorizationService.Setup(x => x.EnsureCurrentUserPermissions(
            It.IsAny<IEnumerable<string>>(), 
            It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        mockSystemSettingsRepository.Setup(x => x.GetValue(sharedKeys.IS_SYSTEM_INITIALIZED, cancellationToken))
            .ReturnsAsync((SystemSetting?)null);

        mockUserRepository.Setup(x => x.UsersExist(cancellationToken))
            .ReturnsAsync(true);

        var result = await service.IsSystemInitialized(cancellationToken);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task SetSystemStateToInitialized_WithValidInput_ShouldUpdateSetting()
    {
        var cancellationToken = new CancellationToken();
        var existingSetting = new SystemSetting { Key = sharedKeys.IS_SYSTEM_INITIALIZED, Value = "False" };

        mockAuthorizationService.Setup(x => x.EnsureCurrentUserPermissions(
            It.IsAny<IEnumerable<string>>(), 
            It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        mockSystemSettingsRepository.Setup(x => x.GetValue(sharedKeys.IS_SYSTEM_INITIALIZED, cancellationToken))
            .ReturnsAsync(existingSetting);

        mockSystemSettingsRepository.Setup(x => x.UpdateSetting(It.IsAny<SystemSetting>()));

        mockRepositoryManager.Setup(x => x.SaveChanges(cancellationToken))
            .Returns(Task.CompletedTask);

        await service.SetSystemStateToInitialized(cancellationToken);

        mockAuthorizationService.Verify(x => x.EnsureCurrentUserPermissions(
            new[] { PermissionCodes.InitializeSystem }, 
            cancellationToken), Times.Once);

        mockSystemSettingsRepository.Verify(x => x.UpdateSetting(It.Is<SystemSetting>(s => s.Value == "True")), Times.Once);
        mockRepositoryManager.Verify(x => x.SaveChanges(cancellationToken), Times.Once);

        mockLogger.Verify(x => x.LogDebug("Starting {MethodName}", "SetSystemStateToInitialized"), Times.Once);
        mockLogger.Verify(x => x.LogInfo("System state set to initialized successfully"), Times.Once);
    }

    [Fact]
    public async Task SetSystemStateToInitialized_WithNullSetting_ShouldCreateNewSetting()
    {
        var cancellationToken = new CancellationToken();

        mockAuthorizationService.Setup(x => x.EnsureCurrentUserPermissions(
            It.IsAny<IEnumerable<string>>(), 
            It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        mockSystemSettingsRepository.Setup(x => x.GetValue(sharedKeys.IS_SYSTEM_INITIALIZED, cancellationToken))
            .ReturnsAsync((SystemSetting?)null);

        mockSystemSettingsRepository.Setup(x => x.UpdateSetting(It.IsAny<SystemSetting>()));

        mockRepositoryManager.Setup(x => x.SaveChanges(cancellationToken))
            .Returns(Task.CompletedTask);

        await service.SetSystemStateToInitialized(cancellationToken);

        mockSystemSettingsRepository.Verify(x => x.UpdateSetting(It.Is<SystemSetting>(s => 
            s.Key == sharedKeys.IS_SYSTEM_INITIALIZED && s.Value == "True")), Times.Once);
    }

    [Fact]
    public async Task IsApplicationSeededAtStartup_WithTrueValue_ShouldReturnTrue()
    {
        var cancellationToken = new CancellationToken();
        var setting = new SystemSetting { Key = sharedKeys.IS_DATABASE_SEEDED_AT_STARTUP, Value = "True" };

        mockAuthorizationService.Setup(x => x.EnsureCurrentUserPermissions(
            It.IsAny<IEnumerable<string>>(), 
            It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        mockSystemSettingsRepository.Setup(x => x.GetValue(sharedKeys.IS_DATABASE_SEEDED_AT_STARTUP, cancellationToken))
            .ReturnsAsync(setting);

        var result = await service.IsApplicationSeededAtStartup(cancellationToken);

        result.Should().BeTrue();

        mockAuthorizationService.Verify(x => x.EnsureCurrentUserPermissions(
            new[] { PermissionCodes.GetSystemSettings }, 
            cancellationToken), Times.Once);
    }

    [Fact]
    public async Task IsApplicationSeededAtStartup_WithFalseValue_ShouldReturnFalse()
    {
        var cancellationToken = new CancellationToken();
        var setting = new SystemSetting { Key = sharedKeys.IS_DATABASE_SEEDED_AT_STARTUP, Value = "False" };

        mockAuthorizationService.Setup(x => x.EnsureCurrentUserPermissions(
            It.IsAny<IEnumerable<string>>(), 
            It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        mockSystemSettingsRepository.Setup(x => x.GetValue(sharedKeys.IS_DATABASE_SEEDED_AT_STARTUP, cancellationToken))
            .ReturnsAsync(setting);

        var result = await service.IsApplicationSeededAtStartup(cancellationToken);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsApplicationSeededAtStartup_WithNullSetting_ShouldReturnFalse()
    {
        var cancellationToken = new CancellationToken();

        mockAuthorizationService.Setup(x => x.EnsureCurrentUserPermissions(
            It.IsAny<IEnumerable<string>>(), 
            It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        mockSystemSettingsRepository.Setup(x => x.GetValue(sharedKeys.IS_DATABASE_SEEDED_AT_STARTUP, cancellationToken))
            .ReturnsAsync((SystemSetting?)null);

        var result = await service.IsApplicationSeededAtStartup(cancellationToken);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task SetSeedStateAtStartupToComplete_WithValidInput_ShouldUpdateSetting()
    {
        var cancellationToken = new CancellationToken();
        var existingSetting = new SystemSetting { Key = sharedKeys.IS_DATABASE_SEEDED_AT_STARTUP, Value = "False" };

        mockAuthorizationService.Setup(x => x.EnsureCurrentUserPermissions(
            It.IsAny<IEnumerable<string>>(), 
            It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        mockSystemSettingsRepository.Setup(x => x.GetValue(sharedKeys.IS_DATABASE_SEEDED_AT_STARTUP, cancellationToken))
            .ReturnsAsync(existingSetting);

        mockSystemSettingsRepository.Setup(x => x.UpdateSetting(It.IsAny<SystemSetting>()));

        mockRepositoryManager.Setup(x => x.SaveChanges(cancellationToken))
            .Returns(Task.CompletedTask);

        await service.SetSeedStateAtStartupToComplete(cancellationToken);

        mockAuthorizationService.Verify(x => x.EnsureCurrentUserPermissions(
            new[] { PermissionCodes.SeedApplication }, 
            cancellationToken), Times.Once);

        mockSystemSettingsRepository.Verify(x => x.UpdateSetting(It.Is<SystemSetting>(s => s.Value == "True")), Times.Once);
        mockRepositoryManager.Verify(x => x.SaveChanges(cancellationToken), Times.Once);

        mockLogger.Verify(x => x.LogDebug("Starting {MethodName}", "SetSeedStateAtStartupToComplete"), Times.Once);
        mockLogger.Verify(x => x.LogInfo("Seed setting set to completed successfully"), Times.Once);
    }

    [Fact]
    public async Task SetSeedStateAtStartupToComplete_WithNullSetting_ShouldCreateNewSetting()
    {
        var cancellationToken = new CancellationToken();

        mockAuthorizationService.Setup(x => x.EnsureCurrentUserPermissions(
            It.IsAny<IEnumerable<string>>(), 
            It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        mockSystemSettingsRepository.Setup(x => x.GetValue(sharedKeys.IS_DATABASE_SEEDED_AT_STARTUP, cancellationToken))
            .ReturnsAsync((SystemSetting?)null);

        mockSystemSettingsRepository.Setup(x => x.UpdateSetting(It.IsAny<SystemSetting>()));

        mockRepositoryManager.Setup(x => x.SaveChanges(cancellationToken))
            .Returns(Task.CompletedTask);

        await service.SetSeedStateAtStartupToComplete(cancellationToken);

        mockSystemSettingsRepository.Verify(x => x.UpdateSetting(It.Is<SystemSetting>(s => 
            s.Key == sharedKeys.IS_DATABASE_SEEDED_AT_STARTUP && s.Value == "True")), Times.Once);
    }

    [Fact]
    public async Task IsMigrationComplete_WithUnauthorizedUser_ShouldThrowException()
    {
        var cancellationToken = new CancellationToken();

        mockAuthorizationService.Setup(x => x.EnsureCurrentUserPermissions(
            It.IsAny<IEnumerable<string>>(), 
            It.IsAny<CancellationToken>()))
            .ThrowsAsync(new AroUnauthorizedException("UNAUTHORIZED", "Insufficient permissions"));

        await Assert.ThrowsAsync<AroUnauthorizedException>(() => 
            service.IsMigrationComplete(cancellationToken));
    }

    [Fact]
    public async Task SetMigrationStateToComplete_WithUnauthorizedUser_ShouldThrowException()
    {
        var cancellationToken = new CancellationToken();

        mockAuthorizationService.Setup(x => x.EnsureCurrentUserPermissions(
            It.IsAny<IEnumerable<string>>(), 
            It.IsAny<CancellationToken>()))
            .ThrowsAsync(new AroUnauthorizedException("UNAUTHORIZED", "Insufficient permissions"));

        await Assert.ThrowsAsync<AroUnauthorizedException>(() => 
            service.SetMigrationStateToComplete(cancellationToken));
    }

    [Fact]
    public async Task IsSystemInitialized_WithUnauthorizedUser_ShouldThrowException()
    {
        var cancellationToken = new CancellationToken();

        mockAuthorizationService.Setup(x => x.EnsureCurrentUserPermissions(
            It.IsAny<IEnumerable<string>>(), 
            It.IsAny<CancellationToken>()))
            .ThrowsAsync(new AroUnauthorizedException("UNAUTHORIZED", "Insufficient permissions"));

        await Assert.ThrowsAsync<AroUnauthorizedException>(() => 
            service.IsSystemInitialized(cancellationToken));
    }

    [Fact]
    public async Task SetSystemStateToInitialized_WithUnauthorizedUser_ShouldThrowException()
    {
        var cancellationToken = new CancellationToken();

        mockAuthorizationService.Setup(x => x.EnsureCurrentUserPermissions(
            It.IsAny<IEnumerable<string>>(), 
            It.IsAny<CancellationToken>()))
            .ThrowsAsync(new AroUnauthorizedException("UNAUTHORIZED", "Insufficient permissions"));

        await Assert.ThrowsAsync<AroUnauthorizedException>(() => 
            service.SetSystemStateToInitialized(cancellationToken));
    }

    [Fact]
    public async Task IsApplicationSeededAtStartup_WithUnauthorizedUser_ShouldThrowException()
    {
        var cancellationToken = new CancellationToken();

        mockAuthorizationService.Setup(x => x.EnsureCurrentUserPermissions(
            It.IsAny<IEnumerable<string>>(), 
            It.IsAny<CancellationToken>()))
            .ThrowsAsync(new AroUnauthorizedException("UNAUTHORIZED", "Insufficient permissions"));

        await Assert.ThrowsAsync<AroUnauthorizedException>(() => 
            service.IsApplicationSeededAtStartup(cancellationToken));
    }

    [Fact]
    public async Task SetSeedStateAtStartupToComplete_WithUnauthorizedUser_ShouldThrowException()
    {
        var cancellationToken = new CancellationToken();

        mockAuthorizationService.Setup(x => x.EnsureCurrentUserPermissions(
            It.IsAny<IEnumerable<string>>(), 
            It.IsAny<CancellationToken>()))
            .ThrowsAsync(new AroUnauthorizedException("UNAUTHORIZED", "Insufficient permissions"));

        await Assert.ThrowsAsync<AroUnauthorizedException>(() => 
            service.SetSeedStateAtStartupToComplete(cancellationToken));
    }
}
