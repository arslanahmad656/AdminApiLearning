using Aro.Common.Application.Shared;

namespace Aro.Admin.Application.Services.PermissionSeeder;

public interface IPermissionSeeder : IService
{
    Task Seed(string jsonFile, CancellationToken cancellationToken = default);
}
