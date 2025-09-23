using Aro.Admin.Domain.Entities;
using Aro.Admin.Domain.Repository;
using Aro.Admin.Infrastructure.Repository.Context;
using Microsoft.EntityFrameworkCore;

namespace Aro.Admin.Infrastructure.Repository.Repositories;

public class RolePermissionRepository(AroAdminApiDbContext dbContext) : RepositoryBase<RolePermission>(dbContext), IRolePermissionRepository
{
    public Task Create(RolePermission rolePermission, CancellationToken cancellationToken = default) => Add(rolePermission, cancellationToken);

    public Task<bool> Exists(Guid roleId, Guid permissionId, CancellationToken cancellationToken = default)
        => FindByCondition(rp => rp.RoleId == roleId && rp.PermissionId == permissionId)
            .AnyAsync(cancellationToken);

    public IQueryable<RolePermission> GetAll() => FindByCondition();
}