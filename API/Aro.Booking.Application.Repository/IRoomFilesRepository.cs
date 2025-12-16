using Aro.Booking.Domain.Entities;
using Aro.Common.Domain.Entities;

namespace Aro.Booking.Application.Repository;

public interface IRoomFilesRepository
{
    IQueryable<RoomFile> GetById(Guid id);

    IQueryable<RoomFile> GetByRoomId(Guid roomId);

    Task Create(RoomFiles roomFiles, CancellationToken cancellationToken = default);

    void Update(RoomFiles roomFiles);

    void Delete(RoomFiles roomFiles);

    public record RoomFile(RoomFiles Entity, Room Room, FileResource File);
}

