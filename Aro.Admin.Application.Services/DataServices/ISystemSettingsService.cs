namespace Aro.Admin.Application.Services.DataServices;

public interface ISystemSettingsService : IService
{
    Task<bool> IsSystemInitialized(CancellationToken cancellationToken = default);

    Task SetSystemStateToInitialized(CancellationToken cancellationToken = default);
}
