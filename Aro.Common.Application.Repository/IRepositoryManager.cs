namespace Aro.Common.Application.Repository;

public interface IRepositoryManager
{
    IAuditTrailRepository AuditTrailRepository { get; }
}
