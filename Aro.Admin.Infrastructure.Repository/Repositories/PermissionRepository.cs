using Aro.Admin.Domain.Entities;
using Aro.Admin.Domain.Repository;
using Aro.Admin.Infrastructure.Repository.Context;
using Microsoft.EntityFrameworkCore;

namespace Aro.Admin.Infrastructure.Repository.Repositories;

public class PermissionRepository(AroAdminApiDbContext dbContext) : RepositoryBase<Permission>(dbContext), IPermissionRepository
{
    
    public Task Create(Permission permission, CancellationToken cancellationToken = default) => Add(permission, cancellationToken);

    public IQueryable<Permission> GetAll() => FindByCondition();

    public Task<Permission?> GetByName(string name, CancellationToken cancellationToken = default)
        => FindByCondition(p => p.Name == name).SingleOrDefaultAsync(cancellationToken);
}