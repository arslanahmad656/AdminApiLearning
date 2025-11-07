using Aro.Common.Domain.Entities;

namespace Aro.Common.Application.Repository;

public interface IRolePermissionRepository
{
    IQueryable<RolePermission> GetAll();

    Task<bool> Exists(Guid roleId, Guid permissionId, CancellationToken cancellationToken = default);

    Task Create(RolePermission rolePermission, CancellationToken cancellationToken = default);
}
