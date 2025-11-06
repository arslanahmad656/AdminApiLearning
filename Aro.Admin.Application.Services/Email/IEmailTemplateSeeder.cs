using Aro.Common.Application.Services;

namespace Aro.Admin.Application.Services.Email;

public interface IEmailTemplateSeeder : IService
{
    Task Seed(string templatesDirectory, CancellationToken ct = default);
}
