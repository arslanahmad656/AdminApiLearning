namespace Aro.Booking.Application.Repository;

public interface IRepositoryManager
{
    IAddressRepository AddressRepository { get; }
    //IContactRepository ContactRepository { get; }
    IGroupRepository GroupRepository { get; }
    IPropertyRepository PropertyRepository { get; }
    IPropertyFilesRepository PropertyFilesRepository { get; }
    IGroupFilesRepository GroupFilesRepository { get; }
    IRoomRepository RoomRepository { get; }
    IRoomFilesRepository RoomFilesRepository { get; }
    IAmenityRepository AmenityRepository { get; }
    IPolicyRepository PolicyRepository { get; }
}
