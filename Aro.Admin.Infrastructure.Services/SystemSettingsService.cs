using Aro.Admin.Application.Services.Authorization;
using Aro.Admin.Application.Services.SystemSettings;
using Aro.Admin.Domain.Repository;
using Aro.Admin.Domain.Shared;
using Aro.Common.Application.Services.LogManager;
using System.Threading;

namespace Aro.Admin.Infrastructure.Services;

public class SystemSettingsService(IRepositoryManager repository, SharedKeys sharedKeys, IAuthorizationService authorizationService, ILogManager<SystemSettingsService> logger) : ISystemSettingsService
{
    private readonly ISystemSettingsRepository settingsRepo = repository.SystemSettingsRepository;
    private readonly IUserRepository userRepository = repository.UserRepository;

    public async Task<bool> IsMigrationComplete(CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Starting {MethodName}", nameof(IsMigrationComplete));
        await authorizationService.EnsureCurrentUserPermissions([PermissionCodes.GetSystemSettings], cancellationToken);
        var setting = await settingsRepo.GetValue(sharedKeys.IS_MIGRATIONS_COMPLETE, cancellationToken).ConfigureAwait(false);

        logger.LogDebug("Settings: @{Settings}", setting ?? new());

        return setting?.Value == true.ToString();
    }

    public async Task SetMigrationStateToComplete(CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Starting {MethodName}", nameof(SetMigrationStateToComplete));

        await authorizationService.EnsureCurrentUserPermissions([PermissionCodes.MigrateDabase], cancellationToken);
        logger.LogDebug("Authorization verified for database migration.");

        var setting = await settingsRepo.GetValue(sharedKeys.IS_MIGRATIONS_COMPLETE, cancellationToken).ConfigureAwait(false)
            ?? new() { Key = sharedKeys.IS_MIGRATIONS_COMPLETE };
        logger.LogDebug("Retrieved or created set migration complete setting");

        setting.Value = true.ToString();
        settingsRepo.UpdateSetting(setting);
        logger.LogDebug("Updated migration complete setting to true");

        await repository.SaveChanges(cancellationToken).ConfigureAwait(false);
        logger.LogInfo("migration state set to completed successfully");

        logger.LogDebug("Completed {MethodName}", nameof(SetMigrationStateToComplete));
    }

    public async Task<bool> IsSystemInitialized(CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Starting {MethodName}", nameof(IsSystemInitialized));
        
        await authorizationService.EnsureCurrentUserPermissions([PermissionCodes.GetSystemSettings], cancellationToken);
        logger.LogDebug("Authorization verified for system initialization check");

        var setting = await settingsRepo.GetValue(sharedKeys.IS_SYSTEM_INITIALIZED, cancellationToken).ConfigureAwait(false);
        logger.LogDebug("Retrieved system initialization setting: {Value}", setting?.Value ?? string.Empty);

        if (setting?.Value == true.ToString())
        {
            logger.LogDebug("System initialization setting is true");
            return true;
        }

        logger.LogDebug("System initialization setting is not true, checking if users exist");
        var usersExist = await userRepository.UsersExist(cancellationToken).ConfigureAwait(false);
        logger.LogDebug("Users exist check result: {UsersExist}", usersExist);

        logger.LogDebug("Completed {MethodName}", nameof(IsSystemInitialized));
        return usersExist;
    }

    public async Task SetSystemStateToInitialized(CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Starting {MethodName}", nameof(SetSystemStateToInitialized));
        
        await authorizationService.EnsureCurrentUserPermissions([PermissionCodes.InitializeSystem], cancellationToken);
        logger.LogDebug("Authorization verified for system initialization");

        var setting = await settingsRepo.GetValue(sharedKeys.IS_SYSTEM_INITIALIZED, cancellationToken).ConfigureAwait(false)
            ?? new() { Key = sharedKeys.IS_SYSTEM_INITIALIZED };
        logger.LogDebug("Retrieved or created system initialization setting");

        setting.Value = true.ToString();
        settingsRepo.UpdateSetting(setting);
        logger.LogDebug("Updated system initialization setting to true");

        await repository.SaveChanges(cancellationToken).ConfigureAwait(false);
        logger.LogInfo("System state set to initialized successfully");
        
        logger.LogDebug("Completed {MethodName}", nameof(SetSystemStateToInitialized));
    }

    public async Task<bool> IsApplicationSeededAtStartup(CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Starting {MethodName}", nameof(IsApplicationSeededAtStartup));
        await authorizationService.EnsureCurrentUserPermissions([PermissionCodes.GetSystemSettings], cancellationToken);
        var setting = await settingsRepo.GetValue(sharedKeys.IS_DATABASE_SEEDED_AT_STARTUP, cancellationToken).ConfigureAwait(false);

        logger.LogDebug("Settings: @{Settings}", setting ?? new());

        return setting?.Value == true.ToString();
    }

    public async Task SetSeedStateAtStartupToComplete(CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Starting {MethodName}", nameof(SetSeedStateAtStartupToComplete));

        await authorizationService.EnsureCurrentUserPermissions([PermissionCodes.SeedApplication], cancellationToken);
        logger.LogDebug("Authorization verified for database seed setting.");

        var setting = await settingsRepo.GetValue(sharedKeys.IS_DATABASE_SEEDED_AT_STARTUP, cancellationToken).ConfigureAwait(false)
            ?? new() { Key = sharedKeys.IS_DATABASE_SEEDED_AT_STARTUP, Value = true.ToString() };
        logger.LogDebug("Retrieved or created seed setting.");

        setting.Value = true.ToString();
        settingsRepo.UpdateSetting(setting);
        logger.LogDebug("Updated seed settings to true.");

        await repository.SaveChanges(cancellationToken).ConfigureAwait(false);
        logger.LogInfo("Seed setting set to completed successfully");

        logger.LogDebug("Completed {MethodName}", nameof(SetSeedStateAtStartupToComplete));
    }
}
