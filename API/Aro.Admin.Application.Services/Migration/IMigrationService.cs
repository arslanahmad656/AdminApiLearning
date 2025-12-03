using Aro.Common.Application.Shared;

namespace Aro.Admin.Application.Services.Migration;

public interface IMigrationService : IService
{
    Task Migrate(bool dropFirst, CancellationToken cancellationToken);
}
