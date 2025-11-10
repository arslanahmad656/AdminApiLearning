using Aro.Common.Application.Repository;
using Aro.Common.Infrastructure.Repository.Context;

namespace Aro.Common.Infrastructure.Repository;

public class RepositoryManager(AroDbContext dbContext) : IRepositoryManager
{
    private readonly Lazy<AuditTrailRepository> auditTrailRepository = new(() => new AuditTrailRepository(dbContext));
    private readonly Lazy<PermissionRepository> permissionRepository = new(() => new PermissionRepository(dbContext));
    private readonly Lazy<RolePermissionRepository> rolePermissionRepository = new(() => new RolePermissionRepository(dbContext));
    private readonly Lazy<RoleRepository> roleRepository = new(() => new RoleRepository(dbContext));
    private readonly Lazy<UserRepository> userRepository = new(() => new UserRepository(dbContext));
    private readonly Lazy<UserRoleRepository> userRoleRepository = new(() => new UserRoleRepository(dbContext));

    public IAuditTrailRepository AuditTrailRepository => auditTrailRepository.Value;

    public IPermissionRepository PermissionRepository => permissionRepository.Value;

    public IRolePermissionRepository RolePermissionRepository => rolePermissionRepository.Value;

    public IRoleRepository RoleRepository => roleRepository.Value;

    public IUserRepository UserRepository => userRepository.Value;

    public IUserRoleRepository UserRoleRepository => userRoleRepository.Value;
}
