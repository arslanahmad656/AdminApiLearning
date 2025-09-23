using Aro.Admin.Domain.Entities;
using Aro.Admin.Domain.Repository;
using Aro.Admin.Infrastructure.Repository.Context;

namespace Aro.Admin.Infrastructure.Repository.Repositories;

public class AuditTrailRepository(AroAdminApiDbContext dbContext) : RepositoryBase<AuditTrail>(dbContext), IAuditTrailRepository
{
    // Implement IAuditTrailRepository members here
}