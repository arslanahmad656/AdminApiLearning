using Aro.Common.Application.Services;

namespace Aro.Admin.Application.Services.Migration;

public interface IMigrationService : IService
{
    Task Migrate(CancellationToken cancellationToken);
}
