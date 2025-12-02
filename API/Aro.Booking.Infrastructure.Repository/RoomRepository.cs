using Aro.Booking.Application.Repository;
using Aro.Booking.Domain.Entities;
using Aro.Common.Infrastructure.Repository;
using Aro.Common.Infrastructure.Repository.Context;
using Microsoft.EntityFrameworkCore;

namespace Aro.Booking.Infrastructure.Repository;

public class RoomRepository(AroDbContext dbContext) : RepositoryBase<Room>(dbContext), IRoomRepository
{
    public IQueryable<Room> GetById(Guid id) => FindByCondition(filter: u => u.Id == id);

    public IQueryable<Room> GetByRoomCode(string roomCode) => FindByCondition(filter: u => u.RoomCode == roomCode);

    //public IQueryable<Room> GetByRoomName(string roomName) => FindByCondition(filter: u => u.RoomName == roomName);

    public IQueryable<Room> GetAll() => FindByCondition();

    public Task<bool> RoomsExist(CancellationToken cancellationToken = default) => FindByCondition()
        .AnyAsync(cancellationToken);

    public Task Create(Room room, CancellationToken cancellationToken = default) => base.Add(room, cancellationToken);

    public new void Update(Room room) => base.Update(room);

    public new void Delete(Room room) => base.Delete(room);

}