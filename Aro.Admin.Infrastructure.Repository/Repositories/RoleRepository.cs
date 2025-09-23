using Aro.Admin.Domain.Entities;
using Aro.Admin.Domain.Repository;
using Aro.Admin.Infrastructure.Repository.Context;
using Microsoft.EntityFrameworkCore;

namespace Aro.Admin.Infrastructure.Repository.Repositories;

public class RoleRepository(AroAdminApiDbContext dbContext) : RepositoryBase<Role>(dbContext), IRoleRepository
{
    public Task Create(Role role, CancellationToken cancellationToken = default) => Add(role, cancellationToken);

    public IQueryable<Role> GetAll() => FindByCondition();

    public Task<Role?> GetByName(string roleName, CancellationToken cancellationToken = default)
        => FindByCondition(r => r.Name == roleName)
            .SingleOrDefaultAsync(cancellationToken);
}