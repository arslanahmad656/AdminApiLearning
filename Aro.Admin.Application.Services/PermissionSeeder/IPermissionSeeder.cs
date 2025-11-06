using Aro.Common.Application.Services;

namespace Aro.Admin.Application.Services.PermissionSeeder;

public interface IPermissionSeeder : IService
{
    Task Seed(string jsonFile, CancellationToken cancellationToken = default);
}
