using Aro.Common.Domain.Entities;

namespace Aro.Common.Application.Repository;

public interface IUserRoleRepository
{
    Task<bool> Exists(Guid userId, Guid roleId, CancellationToken cancellationToken = default);

    IQueryable<UserRole> GetByUserIds(IEnumerable<Guid> userIds);
    
    IQueryable<UserRole> GetByRoleIds(IEnumerable<Guid> roleIds);

    IQueryable<UserRole> GetByUserRoles(IEnumerable<Guid> userIds, IEnumerable<Guid> roleIds);

    Task Create(UserRole userRole, CancellationToken cancellationToken = default);

    void Remove(IEnumerable<UserRole> userRoles);
}
