using Aro.Admin.Domain.Entities;
using Aro.Admin.Domain.Repository;
using Aro.Admin.Infrastructure.Repository.Context;
using Microsoft.EntityFrameworkCore;

namespace Aro.Admin.Infrastructure.Repository.Repositories;

public class UserRoleRepository(AroAdminApiDbContext dbContext) : RepositoryBase<UserRole>(dbContext), IUserRoleRepository
{
    public Task Create(UserRole userRole, CancellationToken cancellationToken = default) => base.Add(userRole, cancellationToken);

    public Task<bool> Exists(Guid userId, Guid roleId, CancellationToken cancellationToken = default) => FindByCondition(
        filter: ur => ur.UserId ==  userId && ur.RoleId == roleId)
        .AnyAsync(cancellationToken);

    public IQueryable<UserRole> GetByRoleIds(IEnumerable<Guid> roleIds)
        => FindByCondition(filter: ur => roleIds.Contains(ur.RoleId));

    public IQueryable<UserRole> GetByUserIds(IEnumerable<Guid> userIds)
        => FindByCondition(filter: ur => userIds.Contains(ur.UserId));

    public IQueryable<UserRole> GetByUserRoles(IEnumerable<Guid> userIds, IEnumerable<Guid> roleIds)
        => FindByCondition(filter: ur => userIds.Contains(ur.UserId) && roleIds.Contains(ur.RoleId));

    public void Remove(IEnumerable<UserRole> userRoles) => base.DeleteRange(userRoles);
}