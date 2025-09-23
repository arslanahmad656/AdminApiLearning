using Aro.Admin.Domain.Entities;
using System.Linq.Expressions;

namespace Aro.Admin.Domain.Repository;

public interface IRepositoryBase<T> where T : class, IEntity
{
    public IQueryable<T> FindByCondition(
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        Func<IQueryable<T>, IQueryable<T>>? include = null,
        bool trackChanges = true);

    Task Add(T entity, CancellationToken cancellation = default);

    void Update(T entity);

    void Delete(T entity);
}
