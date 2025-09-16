namespace Aro.Domain.Repository;

public interface IRepositoryManager
{
    IAuditTrailRepository AuditTrailRepository { get; }
    IIdempotencyRecordRepository IIdempotencyRecordRepository { get; }
    
    IPermissionRepository PermissionRepository { get; }

    IRolePermissionRepository RolePermissionRepository { get; }

    IRoleRepository RoleRepository { get; }

    ISystemSettingsRepository SystemSettingsRepository { get; }

    IUserRepository UserRepository { get; }

    IUserRoleRepository UserRoleRepository { get; }
}
