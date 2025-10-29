using Aro.Admin.Domain.Entities;
using Aro.Admin.Domain.Repository;
using Aro.Admin.Infrastructure.Repository.Context;

namespace Aro.Admin.Infrastructure.Repository.Repositories;

public class EmailTemplateRepository(AroAdminApiDbContext dbContext) : RepositoryBase<EmailTemplate>(dbContext), IEmailTemplateRepository
{
    public IQueryable<EmailTemplate> GetByIdentifier(string identifier) => FindByCondition(filter: et => et.Identifier == identifier);

    public Task Create(EmailTemplate emailTemplate, CancellationToken cancellationToken = default) => base.Add(emailTemplate, cancellationToken);
}
