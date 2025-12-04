using Aro.Booking.Domain.Entities;

namespace Aro.Booking.Application.Repository;

public interface IPropertyRepository
{
    IQueryable<Property> GetById(Guid id);

    IQueryable<Property> GetByGroupAndId(Guid groupId, Guid propertyId);

    IQueryable<Property> GetByGroupId(Guid groupId);

    IQueryable<Property> GetByNameAndGroupId(string propertyName, Guid? groupId);

    Task Create(Property property, CancellationToken cancellationToken = default);

    void Update(Property property);

    void Delete(Property property);
}
