using Aro.Admin.Application.Repository;
using Aro.Admin.Domain.Entities;
using Aro.Common.Infrastructure.Repository;
using Aro.Common.Infrastructure.Repository.Context;

namespace Aro.Admin.Infrastructure.Repository;

public class EmailTemplateRepository(AroDbContext dbContext) : RepositoryBase<EmailTemplate>(dbContext), IEmailTemplateRepository
{
    public IQueryable<EmailTemplate> GetByIdentifier(string identifier) => FindByCondition(filter: et => et.Identifier == identifier);

    public Task Create(EmailTemplate emailTemplate, CancellationToken cancellationToken = default) => base.Add(emailTemplate, cancellationToken);
}
