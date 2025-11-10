namespace Aro.Common.Application.Repository;

public interface IUnitOfWork
{
    Task SaveChanges(CancellationToken cancellationToken = default);
}
