using Aro.Booking.Application.Repository;
using Aro.Booking.Domain.Entities;
using Aro.Common.Infrastructure.Repository;
using Aro.Common.Infrastructure.Repository.Context;

namespace Aro.Booking.Infrastructure.Repository;

public class AddressRepository(AroDbContext dbContext) : RepositoryBase<Address>(dbContext), IAddressRepository
{
    public IQueryable<Address> GetById(Guid id) => FindByCondition(filter: a => a.Id == id);

    public Task Create(Address address, CancellationToken cancellationToken = default) => base.Add(address, cancellationToken);

    public new void Update(Address address) => base.Update(address);

    public new void Delete(Address address) => base.Delete(address);
}

