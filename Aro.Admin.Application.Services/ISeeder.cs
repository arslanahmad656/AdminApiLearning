namespace Aro.Admin.Application.Services;

public interface ISeeder
{
    Task Seed(string jsonFile, CancellationToken cancellationToken = default);
}
