using Aro.Admin.Domain.Entities;

namespace Aro.Admin.Domain.Repository;

public interface IEmailTemplateRepository
{
    IQueryable<EmailTemplate> GetByIdentifier(string identifier);
    Task Create(EmailTemplate emailTemplate, CancellationToken cancellationToken = default);
}
