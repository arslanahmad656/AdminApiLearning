using Aro.Booking.Application.Repository;
using Aro.Common.Infrastructure.Repository.Context;

namespace Aro.Booking.Infrastructure.Repository;

public class RepositoryManager(AroDbContext dbContext) : IRepositoryManager
{
    private readonly Lazy<AddressRepository> addressRepository = new(() => new AddressRepository(dbContext));
    //private readonly Lazy<ContactRepository> contactRepository = new(() => new ContactRepository(dbContext));
    private readonly Lazy<GroupRepository> groupRepository = new(() => new GroupRepository(dbContext));
    private readonly Lazy<PropertyRepository> propertyRepository = new(() => new PropertyRepository(dbContext));
    private readonly Lazy<PropertyFilesRepository> propertyFilesRepository = new(() => new PropertyFilesRepository(dbContext));
    private readonly Lazy<RoomRepository> roomRepository = new(() => new RoomRepository(dbContext));
    private readonly Lazy<AmenityRepository> amenityRepository = new(() => new AmenityRepository(dbContext));
    private readonly Lazy<PolicyRepository> policyRepository = new(() => new PolicyRepository(dbContext));
    private readonly Lazy<GroupFilesRepository> groupFilesRepository = new(() => new GroupFilesRepository(dbContext));

    public IAddressRepository AddressRepository => addressRepository.Value;
    //public IContactRepository ContactRepository => contactRepository.Value;
    public IGroupRepository GroupRepository => groupRepository.Value;
    public IPropertyRepository PropertyRepository => propertyRepository.Value;
    public IPropertyFilesRepository PropertyFilesRepository => propertyFilesRepository.Value;
    public IRoomRepository RoomRepository => roomRepository.Value;
    public IAmenityRepository AmenityRepository => amenityRepository.Value;
    public IPolicyRepository PolicyRepository => policyRepository.Value;
    public IGroupFilesRepository GroupFilesRepository => groupFilesRepository.Value;
}
