using Aro.Booking.Application.Repository;
using Aro.Common.Infrastructure.Repository.Context;

namespace Aro.Booking.Infrastructure.Repository;

public class RepositoryManager(AroDbContext dbContext) : IRepositoryManager
{
    private readonly Lazy<GroupRepository> groupRepository = new(() => new GroupRepository(dbContext));
    private readonly Lazy<PropertyRepository> propertyRepository = new(() => new PropertyRepository(dbContext));

    public IGroupRepository GroupRepository => groupRepository.Value;
    public IPropertyRepository PropertyRepository => propertyRepository.Value;
}
