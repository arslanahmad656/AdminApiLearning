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

    public IAuditTrailRepository AuditTrailRepository => auditTrailRepository.Value;

    public IIdempotencyRecordRepository IIdempotencyRecordRepository => idempotencyRecordRepository.Value;

    public IPermissionRepository PermissionRepository => permissionRepository.Value;

    public IRolePermissionRepository RolePermissionRepository => rolePermissionRepository.Value;

    public IRoleRepository RoleRepository => roleRepository.Value;

    public ISystemSettingsRepository SystemSettingsRepository => systemSettingsRepository.Value;

    public IUserRepository UserRepository => userRepository.Value;

    public IUserRoleRepository UserRoleRepository => userRoleRepository.Value;

    public Task SaveChanges(CancellationToken cancellationToken = default) => dbContext.SaveChangesAsync(cancellationToken);
}