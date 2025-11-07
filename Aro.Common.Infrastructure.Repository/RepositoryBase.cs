using Aro.Common.Application.Repository;
using Aro.Common.Domain.Entities;
using Aro.Common.Infrastructure.Repository.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Aro.Common.Infrastructure.Repository;

public abstract class RepositoryBase<T>(AroDbContext dbContext)
    : IRepositoryBase<T> where T : class, IEntity
{
    private readonly DbSet<T> set = dbContext.Set<T>();

    protected AroDbContext DbContext => dbContext;

    public Task Add(T entity, CancellationToken cancellation = default) => set.AddAsync(entity, cancellation).AsTask();

    public void Delete(T entity) => set.Remove(entity);

    public void DeleteRange(IEnumerable<T> entities) => set.RemoveRange(entities);

    public IQueryable<T> FindByCondition
    (
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        Func<IQueryable<T>, IQueryable<T>>? include = null,
        bool trackChanges = true
    )
    {
        IQueryable<T> query = set.AsQueryable();

        if (!trackChanges)
        {
            query = query.AsNoTracking();
        }

        if (include != null)
        {
            query = include(query);
        }

        if (filter != null)
        {
            query = query.Where(filter);
        }

        if (orderBy != null)
        {
            query = orderBy(query);
        }

        return query;
    }

    public void Update(T entity) => set.Update(entity);
}
