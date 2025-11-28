using Aro.Booking.Application.Repository;
using Aro.Booking.Domain.Entities;
using Aro.Common.Infrastructure.Repository;
using Aro.Common.Infrastructure.Repository.Context;
using Microsoft.EntityFrameworkCore;

namespace Aro.Booking.Infrastructure.Repository;

public class GroupRepository(AroDbContext dbContext) : RepositoryBase<Group>(dbContext), IGroupRepository
{
    public IQueryable<Group> GetById(Guid id) => FindByCondition(filter: u => u.Id == id);

    public IQueryable<Group> GetAll() => FindByCondition();

    public Task<bool> GroupsExist(CancellationToken cancellationToken = default) => FindByCondition()
        .AnyAsync(cancellationToken);

    public Task Create(Group group, CancellationToken cancellationToken = default) => base.Add(group, cancellationToken);

    public new void Update(Group group) => base.Update(group);

}