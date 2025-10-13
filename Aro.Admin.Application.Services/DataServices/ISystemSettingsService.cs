namespace Aro.Admin.Application.Services.DataServices;

public interface ISystemSettingsService : IService
{
    Task<bool> IsSystemInitialized(CancellationToken cancellationToken = default);

    Task SetSystemStateToInitialized(CancellationToken cancellationToken = default);

    Task<bool> IsMigrationComplete(CancellationToken cancellationToken = default);

    Task SetMigrationStateToComplete(CancellationToken cancellationToken = default);

    Task<bool> IsApplicationSeededAtStartup(CancellationToken cancellationToken = default);

    Task SetSeedStateAtStartupToComplete(CancellationToken cancellationToken = default);
}
