namespace Aro.Admin.Application.Repository;

public interface IRepositoryManager
{
    IIdempotencyRecordRepository IIdempotencyRecordRepository { get; }
    
    IPermissionRepository PermissionRepository { get; }

    IRolePermissionRepository RolePermissionRepository { get; }

    IRoleRepository RoleRepository { get; }

    ISystemSettingsRepository SystemSettingsRepository { get; }

    IUserRoleRepository UserRoleRepository { get; }

    IRefreshTokenRepository RefreshTokenRepository { get; }

    IPasswordResetTokenRepository PasswordResetTokenRepository { get; }

    IEmailTemplateRepository EmailTemplateRepository { get; }

    IPasswordHistoryRepository PasswordHistoryRepository { get; }
}
