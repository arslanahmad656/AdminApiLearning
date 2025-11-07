using Aro.Common.Domain.Entities;

namespace Aro.Common.Application.Repository;

public interface IRoleRepository
{
    Task<Role?> GetByName(string roleName, CancellationToken cancellationToken = default);

    IQueryable<Role> GetByNames(IEnumerable<string> roleNames);

    IQueryable<Role> GetAll();

    Task Create(Role role, CancellationToken cancellationToken = default);
}
