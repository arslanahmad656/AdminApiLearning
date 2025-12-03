namespace Aro.Booking.Application.Repository;

public interface IRepositoryManager
{
    IAddressRepository AddressRepository { get; }
    IContactRepository ContactRepository { get; }
    IGroupRepository GroupRepository { get; }
    IPropertyRepository PropertyRepository { get; }
    IRoomRepository RoomRepository { get; }
    IAmenityRepository AmenityRepository { get; }
}
