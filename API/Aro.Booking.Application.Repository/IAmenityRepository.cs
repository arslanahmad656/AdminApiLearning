using Aro.Booking.Domain.Entities;

namespace Aro.Booking.Application.Repository;

public interface IAmenityRepository
{
    IQueryable<Amenity> GetAll();

    IQueryable<Amenity> GetById(Guid Id);

    Task Create(Amenity amenity, CancellationToken cancellationToken = default);

    void Update(Amenity amenity);

    void Delete(Amenity amenity);
}
