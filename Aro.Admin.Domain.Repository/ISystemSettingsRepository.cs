using Aro.Admin.Domain.Entities;

namespace Aro.Admin.Domain.Repository;

public interface ISystemSettingsRepository
{
    Task<SystemSetting?> GetValue(string key, CancellationToken cancellationToken = default);

    void UpdateSetting(SystemSetting setting);
}
