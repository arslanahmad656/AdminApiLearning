namespace Aro.Admin.Application.Services;

public interface IPermissionSeeder : IService
{
    Task Seed(string jsonFile, CancellationToken cancellationToken = default);
}
