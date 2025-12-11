using Aro.Booking.Application.Repository;
using Aro.Booking.Domain.Entities;
using Aro.Common.Infrastructure.Repository;
using Aro.Common.Infrastructure.Repository.Context;

namespace Aro.Booking.Infrastructure.Repository;

public class PolicyRepository(AroDbContext dbContext) : RepositoryBase<Policy>(dbContext), IPolicyRepository
{
    public IQueryable<Policy> GetById(Guid id) => FindByCondition(filter: p => p.Id == id);

    public IQueryable<Policy> GetAll() => FindByCondition();

    public Task Create(Policy policy, CancellationToken cancellationToken = default) => base.Add(policy, cancellationToken);

    public new void Update(Policy policy) => base.Update(policy);

    public new void Delete(Policy policy) => base.Delete(policy);
}

