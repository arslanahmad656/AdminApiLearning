using Aro.Admin.Application.Services;
using Aro.Admin.Application.Services.DataServices;
using Aro.Admin.Domain.Repository;
using Aro.Admin.Domain.Shared;

namespace Aro.Admin.Infrastructure.Services;

public class SystemSettingsService(IRepositoryManager repository, SharedKeys sharedKeys, IAuthorizationService authorizationService, ILogManager<SystemSettingsService> logger) : ISystemSettingsService
{
    private readonly ISystemSettingsRepository settingsRepo = repository.SystemSettingsRepository;
    private readonly IUserRepository userRepository = repository.UserRepository;
    
    public async Task<bool> IsSystemInitialized(CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Starting {MethodName}", nameof(IsSystemInitialized));
        
        await authorizationService.EnsureCurrentUserPermissions([PermissionCodes.GetSystemSettings, PermissionCodes.CreateUser], cancellationToken);
        logger.LogDebug("Authorization verified for system initialization check");

        var setting = await settingsRepo.GetValue(sharedKeys.IS_SYSTEM_INITIALIZED, cancellationToken).ConfigureAwait(false);
        logger.LogDebug("Retrieved system initialization setting: {Value}", setting?.Value);

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
}
