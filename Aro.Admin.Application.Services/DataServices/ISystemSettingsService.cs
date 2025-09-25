namespace Aro.Admin.Application.Services.DataServices;

public interface ISystemSettingsService
{
    Task<bool> IsSystemInitialized(CancellationToken cancellationToken = default);

    Task SetSystemStateToInitialized(CancellationToken cancellationToken = default);
}
