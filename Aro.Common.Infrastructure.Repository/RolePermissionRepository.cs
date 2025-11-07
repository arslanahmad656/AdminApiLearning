using Aro.Common.Application.Repository;
using Aro.Common.Domain.Entities;
using Aro.Common.Infrastructure.Repository.Context;
using Microsoft.EntityFrameworkCore;

namespace Aro.Common.Infrastructure.Repository;

public class RolePermissionRepository(AroDbContext dbContext) : RepositoryBase<RolePermission>(dbContext), IRolePermissionRepository
{
    public Task Create(RolePermission rolePermission, CancellationToken cancellationToken = default) => Add(rolePermission, cancellationToken);

    public Task<bool> Exists(Guid roleId, Guid permissionId, CancellationToken cancellationToken = default)
        => FindByCondition(rp => rp.RoleId == roleId && rp.PermissionId == permissionId)
            .AnyAsync(cancellationToken);

    public IQueryable<RolePermission> GetAll() => FindByCondition();
}