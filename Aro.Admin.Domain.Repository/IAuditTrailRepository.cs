using Aro.Admin.Domain.Entities;

namespace Aro.Admin.Domain.Repository;

public interface IAuditTrailRepository
{
    Task Create(AuditTrail trail, CancellationToken cancellationToken = default);
}
