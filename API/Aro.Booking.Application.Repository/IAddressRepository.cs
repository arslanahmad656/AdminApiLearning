using Aro.Booking.Domain.Entities;

namespace Aro.Booking.Application.Repository;

public interface IAddressRepository
{
    IQueryable<Address> GetById(Guid id);

    Task Create(Address address, CancellationToken cancellationToken = default);

    void Update(Address address);

    void Delete(Address address);
}

