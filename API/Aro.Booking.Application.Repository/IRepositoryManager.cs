namespace Aro.Booking.Application.Repository;

public interface IRepositoryManager
{
    IGroupRepository GroupRepository { get; }
    IPropertyRepository PropertyRepository { get; }
}
