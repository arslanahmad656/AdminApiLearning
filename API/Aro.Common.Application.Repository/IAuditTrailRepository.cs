using Aro.Common.Domain.Entities;

namespace Aro.Common.Application.Repository;


public interface IAuditTrailRepository
{
    Task Create(AuditTrail trail, CancellationToken cancellationToken = default);
}
