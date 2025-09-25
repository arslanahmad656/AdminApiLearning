using Aro.Admin.Application.Services.DataServices;
using Aro.Admin.Domain.Repository;
using Aro.Admin.Domain.Shared;

namespace Aro.Admin.Infrastructure.Services;

public class SystemSettingsService(IRepositoryManager repository, SharedKeys sharedKeys) : ISystemSettingsService
{
    private readonly ISystemSettingsRepository settingsRepo = repository.SystemSettingsRepository;
    private readonly IUserRepository userRepository = repository.UserRepository;
    
    public async Task<bool> IsSystemInitialized(CancellationToken cancellationToken = default)
    {
        var setting = await settingsRepo.GetValue(sharedKeys.IS_SYSTEM_INITIALIZED, cancellationToken).ConfigureAwait(false);

        if (setting?.Value == true.ToString())
        {
            // if setting has been set to true, it means that the system has already been initialized.
            return true;
        }

        // otherwise, the system is considered to be initialized even if the setting is false when at least one user exist.
        var usersExist = await userRepository.UsersExist(cancellationToken).ConfigureAwait(false);

        return usersExist;
    }

    public async Task SetSystemStateToInitialized(CancellationToken cancellationToken = default)
    {
        var setting = await settingsRepo.GetValue(sharedKeys.IS_SYSTEM_INITIALIZED, cancellationToken).ConfigureAwait(false)
            ?? new() { Key = sharedKeys.IS_SYSTEM_INITIALIZED };

        setting.Value = true.ToString();
        settingsRepo.UpdateSetting(setting);

        await repository.SaveChanges(cancellationToken).ConfigureAwait(false);
    }
}
