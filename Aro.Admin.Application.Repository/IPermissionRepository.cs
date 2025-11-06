using Aro.Admin.Domain.Entities;

namespace Aro.Admin.Application.Repository;

public interface IPermissionRepository
{
    Task<Permission?> GetByName(string name, CancellationToken cancellationToken = default);

    IQueryable<Permission> GetAll();

    Task Create(Permission permission, CancellationToken cancellationToken = default);
}
