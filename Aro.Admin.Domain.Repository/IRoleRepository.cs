using Aro.Admin.Domain.Entities;

namespace Aro.Admin.Domain.Repository;

public interface IRoleRepository
{
    public Task<Role?> GetByName(string roleName, CancellationToken cancellationToken = default);

    public IQueryable<Role> GetAll();

    public Task Create(Role role, CancellationToken cancellationToken = default);
}
