using Aro.Admin.Domain.Entities;

namespace Aro.Admin.Application.Repository;

public interface IAuditTrailRepository
{
    Task Create(AuditTrail trail, CancellationToken cancellationToken = default);
}
