namespace Aro.Common.Application.Repository;

public interface IRepositoryManager
{
    IAuditTrailRepository AuditTrailRepository { get; }

    IUserRepository UserRepository { get; }

    IPermissionRepository PermissionRepository { get; }

    IRolePermissionRepository RolePermissionRepository { get; }

    IRoleRepository RoleRepository { get; }

    IUserRoleRepository UserRoleRepository { get; }

    IFileResourceRepository FileResourceRepository { get; }
}
