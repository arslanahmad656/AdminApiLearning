using Aro.Common.Application.Repository;
using Aro.Common.Domain.Entities;
using Aro.Common.Infrastructure.Repository.Context;
using Microsoft.EntityFrameworkCore;

namespace Aro.Common.Infrastructure.Repository;

public class PermissionRepository(AroDbContext dbContext) : RepositoryBase<Permission>(dbContext), IPermissionRepository
{

    public Task Create(Permission permission, CancellationToken cancellationToken = default) => Add(permission, cancellationToken);

    public IQueryable<Permission> GetAll() => FindByCondition();

    public Task<Permission?> GetByName(string name, CancellationToken cancellationToken = default)
        => FindByCondition(p => p.Name == name).SingleOrDefaultAsync(cancellationToken);
}