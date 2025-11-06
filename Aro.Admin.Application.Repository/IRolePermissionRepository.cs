using Aro.Admin.Domain.Entities;

namespace Aro.Admin.Application.Repository;

public interface IRolePermissionRepository
{
    IQueryable<RolePermission> GetAll();

    Task<bool> Exists(Guid roleId, Guid permissionId, CancellationToken cancellationToken = default);

    Task Create(RolePermission rolePermission, CancellationToken cancellationToken = default);
}
