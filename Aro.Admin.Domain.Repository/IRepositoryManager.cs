namespace Aro.Admin.Domain.Repository;

public interface IRepositoryManager // also acts as a Unit of Work
{
    IAuditTrailRepository AuditTrailRepository { get; }

    IIdempotencyRecordRepository IIdempotencyRecordRepository { get; }
    
    IPermissionRepository PermissionRepository { get; }

    IRolePermissionRepository RolePermissionRepository { get; }

    IRoleRepository RoleRepository { get; }

    ISystemSettingsRepository SystemSettingsRepository { get; }

    IUserRepository UserRepository { get; }

    IUserRoleRepository UserRoleRepository { get; }

    IRefreshTokenRepository RefreshTokenRepository { get; }

    IPasswordResetTokenRepository PasswordResetTokenRepository { get; }

    IEmailTemplateRepository EmailTemplateRepository { get; }

    IPasswordHistoryRepository PasswordHistoryRepository { get; }

    IGroupRepository GroupRepository { get; }

    Task SaveChanges(CancellationToken cancellationToken = default);
}
