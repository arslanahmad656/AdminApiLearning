using Aro.Admin.Domain.Repository;
using Aro.Admin.Infrastructure.Repository.Context;

namespace Aro.Admin.Infrastructure.Repository.Repositories;

public class RepositoryManager(AroAdminApiDbContext dbContext) : IRepositoryManager
{
    private readonly Lazy<AuditTrailRepository> auditTrailRepository = new(() => new AuditTrailRepository(dbContext));
    private readonly Lazy<IdempotencyRecordRepository> idempotencyRecordRepository = new(() => new IdempotencyRecordRepository(dbContext));
    private readonly Lazy<PermissionRepository> permissionRepository = new(() => new PermissionRepository(dbContext));
    private readonly Lazy<RolePermissionRepository> rolePermissionRepository = new(() => new RolePermissionRepository(dbContext));
    private readonly Lazy<RoleRepository> roleRepository = new(() => new RoleRepository(dbContext));
    private readonly Lazy<SystemSettingsRepository> systemSettingsRepository = new(() => new SystemSettingsRepository(dbContext));
    private readonly Lazy<UserRepository> userRepository = new(() => new UserRepository(dbContext));
    private readonly Lazy<UserRoleRepository> userRoleRepository = new(() => new UserRoleRepository(dbContext));
    private readonly Lazy<RefreshTokenRepository> refreshTokenRepository = new(() => new RefreshTokenRepository(dbContext));
    private readonly Lazy<PasswordResetTokenRepository> passwordResetTokenRepository = new(() => new PasswordResetTokenRepository(dbContext));
    private readonly Lazy<EmailTemplateRepository> emailTemplateRepository = new(() => new EmailTemplateRepository(dbContext));
    private readonly Lazy<PasswordHistoryRepository> passwordHistoryRepository = new(() => new PasswordHistoryRepository(dbContext));

    public IAuditTrailRepository AuditTrailRepository => auditTrailRepository.Value;

    public IIdempotencyRecordRepository IIdempotencyRecordRepository => idempotencyRecordRepository.Value;

    public IPermissionRepository PermissionRepository => permissionRepository.Value;

    public IRolePermissionRepository RolePermissionRepository => rolePermissionRepository.Value;

    public IRoleRepository RoleRepository => roleRepository.Value;

    public ISystemSettingsRepository SystemSettingsRepository => systemSettingsRepository.Value;

    public IUserRepository UserRepository => userRepository.Value;

    public IUserRoleRepository UserRoleRepository => userRoleRepository.Value;

    public IRefreshTokenRepository RefreshTokenRepository => refreshTokenRepository.Value;

    public IPasswordResetTokenRepository PasswordResetTokenRepository => passwordResetTokenRepository.Value;

    public IEmailTemplateRepository EmailTemplateRepository => emailTemplateRepository.Value;

    public IPasswordHistoryRepository PasswordHistoryRepository => passwordHistoryRepository.Value;

    public Task SaveChanges(CancellationToken cancellationToken = default) => dbContext.SaveChangesAsync(cancellationToken);
}