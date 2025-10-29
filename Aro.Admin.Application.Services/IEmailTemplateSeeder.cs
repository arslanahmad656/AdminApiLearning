namespace Aro.Admin.Application.Services;

public interface IEmailTemplateSeeder : IService
{
    Task Seed(string templatesDirectory, CancellationToken ct = default);
}
