using Aro.Common.Application.Shared;

namespace Aro.Admin.Application.Services.Email;

public interface IEmailTemplateSeeder : IService
{
    Task Seed(string templatesDirectory, CancellationToken ct = default);
}
