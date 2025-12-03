using Aro.Booking.Application.Repository;
using Aro.Booking.Domain.Entities;
using Aro.Common.Infrastructure.Repository;
using Aro.Common.Infrastructure.Repository.Context;

namespace Aro.Booking.Infrastructure.Repository;

public class AmenityRepository(AroDbContext dbContext) : RepositoryBase<Amenity>(dbContext), IAmenityRepository
{
    public IQueryable<Amenity> GetById(Guid id) => FindByCondition(filter: u => u.Id == id);

    public IQueryable<Amenity> GetAll() => FindByCondition();

    public Task Create(Amenity amenity, CancellationToken cancellationToken = default) => base.Add(amenity, cancellationToken);

    public new void Update(Amenity amenity) => base.Update(amenity);

    public new void Delete(Amenity amenity) => base.Delete(amenity);

}