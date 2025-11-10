using Aro.Common.Application.Repository;
using Aro.Common.Domain.Entities;
using Aro.Common.Infrastructure.Repository.Context;
using Microsoft.EntityFrameworkCore;

namespace Aro.Common.Infrastructure.Repository;

public class UserRoleRepository(AroDbContext dbContext) : RepositoryBase<UserRole>(dbContext), IUserRoleRepository
{
    public Task Create(UserRole userRole, CancellationToken cancellationToken = default) => Add(userRole, cancellationToken);

    public Task<bool> Exists(Guid userId, Guid roleId, CancellationToken cancellationToken = default) => FindByCondition(
        filter: ur => ur.UserId == userId && ur.RoleId == roleId)
        .AnyAsync(cancellationToken);

    public IQueryable<UserRole> GetByRoleIds(IEnumerable<Guid> roleIds)
        => FindByCondition(filter: ur => roleIds.Contains(ur.RoleId));

    public IQueryable<UserRole> GetByUserIds(IEnumerable<Guid> userIds)
        => FindByCondition(filter: ur => userIds.Contains(ur.UserId));

    public IQueryable<UserRole> GetByUserRoles(IEnumerable<Guid> userIds, IEnumerable<Guid> roleIds)
        => FindByCondition(filter: ur => userIds.Contains(ur.UserId) && roleIds.Contains(ur.RoleId));

    public void Remove(IEnumerable<UserRole> userRoles) => DeleteRange(userRoles);
}