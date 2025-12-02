using Aro.Booking.Domain.Entities;

namespace Aro.Booking.Application.Repository;

public interface IRoomRepository
{
    IQueryable<Room> GetById(Guid Id);

    IQueryable<Room> GetByRoomCode(string roomCode);

    //IQueryable<Room> GetByRoomName(string roomName);

    IQueryable<Room> GetAll();

    Task<bool> RoomsExist(CancellationToken cancellationToken = default);

    Task Create(Room room, CancellationToken cancellationToken = default);

    void Update(Room room);

    void Delete(Room room);
}
