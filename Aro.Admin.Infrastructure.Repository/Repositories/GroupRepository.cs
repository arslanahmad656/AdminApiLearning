using Aro.Admin.Domain.Entities;
using Aro.Admin.Domain.Repository;
using Aro.Admin.Infrastructure.Repository.Context;
using Microsoft.EntityFrameworkCore;

namespace Aro.Admin.Infrastructure.Repository.Repositories;

public class GroupRepository(AroAdminApiDbContext dbContext) : RepositoryBase<Group>(dbContext), IGroupRepository
{
    public IQueryable<Group> GetById(Guid id) => FindByCondition(filter: u => u.Id == id);

    public IQueryable<Group> GetAll() => FindByCondition();

    public Task<bool> GroupsExist(CancellationToken cancellationToken = default) => FindByCondition()
        .AnyAsync(cancellationToken);

    public Task Create(Group group, CancellationToken cancellationToken = default) => base.Add(group, cancellationToken);

    public new void Update(Group group) => base.Update(group);

    public new void Delete(Group group) => base.Delete(group);
   
}