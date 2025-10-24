using Aro.Admin.Application.Services.DTOs.ServiceParameters.PasswordReset;

namespace Aro.Admin.Application.Services.DataServices;

public interface IEmailTemplateService : IService
{
    Task<EmailTemplateDto> GetByIdentifier(string identifier, CancellationToken cancellationToken = default);
    Task<EmailTemplateDto> Add(EmailTemplateDto emailTemplate, CancellationToken cancellationToken = default);
    Task<IEnumerable<EmailTemplateDto>> AddRange(IEnumerable<EmailTemplateDto> emailTemplates, CancellationToken cancellationToken = default);
}
