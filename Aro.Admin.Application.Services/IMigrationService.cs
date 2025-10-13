namespace Aro.Admin.Application.Services;

public interface IMigrationService : IService
{
    Task Migrate(CancellationToken cancellationToken);
}
