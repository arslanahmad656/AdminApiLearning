using Aro.Common.Application.Repository;
using Aro.Common.Domain.Entities;
using Aro.Common.Infrastructure.Repository.Context;
using Microsoft.EntityFrameworkCore;

namespace Aro.Common.Infrastructure.Repository;

public class RoleRepository(AroDbContext dbContext) : RepositoryBase<Role>(dbContext), IRoleRepository
{
    public Task Create(Role role, CancellationToken cancellationToken = default) => Add(role, cancellationToken);

    public IQueryable<Role> GetAll() => FindByCondition();

    public Task<Role?> GetByName(string roleName, CancellationToken cancellationToken = default)
        => FindByCondition(r => r.Name == roleName)
            .SingleOrDefaultAsync(cancellationToken);

    public IQueryable<Role> GetByNames(IEnumerable<string> roleNames) => FindByCondition(filter: r => roleNames.Contains(r.Name));
}