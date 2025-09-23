using Aro.Admin.Domain.Entities;
using Aro.Admin.Domain.Repository;
using Aro.Admin.Infrastructure.Repository.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Aro.Admin.Infrastructure.Repository.Repositories;

public abstract class RepositoryBase<T>(AroAdminApiDbContext dbContext)
    : IRepositoryBase<T> where T : class, IEntity
{
    private readonly  DbSet<T> set = dbContext.Set<T>();

    protected AroAdminApiDbContext DbContext => dbContext;

    public Task Add(T entity, CancellationToken cancellation = default) => set.AddAsync(entity, cancellation).AsTask();

    public void Delete(T entity) => set.Remove(entity);

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
