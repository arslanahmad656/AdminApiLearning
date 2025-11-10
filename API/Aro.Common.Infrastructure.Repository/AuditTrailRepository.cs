using Aro.Common.Application.Repository;
using Aro.Common.Domain.Entities;
using Aro.Common.Infrastructure.Repository.Context;

namespace Aro.Common.Infrastructure.Repository;

public class AuditTrailRepository(AroDbContext dbContext) : RepositoryBase<AuditTrail>(dbContext), IAuditTrailRepository
{
    public Task Create(AuditTrail trail, CancellationToken cancellationToken = default) => Add(trail, cancellationToken);
}