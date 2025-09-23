namespace Aro.Admin.Application.Services;

public interface IMigrationService
{
    Task Migrate(CancellationToken cancellationToken);
}
