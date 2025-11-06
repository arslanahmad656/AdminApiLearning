using Aro.Common.Application.Services;

namespace Aro.Admin.Application.Services.Email;

public interface IEmailTemplateService : IService
{
    public Task<EmailTemplateDto> GetPasswordResetLinkEmail(string name, string resetUrl, int tokenExpiryMinutes, CancellationToken cancellationToken = default);
    Task<EmailTemplateDto> Add(EmailTemplateDto emailTemplate, CancellationToken cancellationToken = default);
    Task<IEnumerable<EmailTemplateDto>> AddRange(IEnumerable<EmailTemplateDto> emailTemplates, CancellationToken cancellationToken = default);
}
