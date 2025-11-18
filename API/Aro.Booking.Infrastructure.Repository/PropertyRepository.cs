using Aro.Booking.Application.Repository;
using Aro.Booking.Domain.Entities;
using Aro.Common.Infrastructure.Repository;
using Aro.Common.Infrastructure.Repository.Context;

namespace Aro.Booking.Infrastructure.Repository;

public class PropertyRepository(AroDbContext dbContext) : RepositoryBase<Property>(dbContext), IPropertyRepository
{
    public IQueryable<Property> GetById(Guid id) => FindByCondition(filter: p => p.Id == id);

    public IQueryable<Property> GetByGroupId(Guid groupId) => FindByCondition(filter: p => p.GroupId == groupId && p.IsActive);

    public IQueryable<Property> GetByNameAndGroupId(string propertyName, Guid? groupId) =>
        FindByCondition(filter: p => p.PropertyName == propertyName && p.GroupId == groupId);

    public Task Create(Property property, CancellationToken cancellationToken = default) => base.Add(property, cancellationToken);

    public new void Update(Property property) => base.Update(property);

    public new void Delete(Property property) => base.Delete(property);
}
