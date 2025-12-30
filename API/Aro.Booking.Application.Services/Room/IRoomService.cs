using Aro.Common.Application.Shared;

namespace Aro.Booking.Application.Services.Room;

public interface IRoomService : IService
{
    Task<CreateRoomResponse> CreateRoom(CreateRoomDto room, CancellationToken cancellationToken = default);

    Task<GetRoomsResponse> GetRooms(GetRoomsDto query, CancellationToken cancellationToken = default);

    Task<GetRoomResponse> GetRoomById(GetRoomDto query, CancellationToken cancellationToken = default);

    Task<PatchRoomResponse> PatchRoom(PatchRoomDto room, CancellationToken cancellationToken = default);

    Task<DeleteRoomResponse> DeleteRoom(DeleteRoomDto room, CancellationToken cancellationToken = default);

    Task<ActivateRoomResponse> ActivateRoom(ActivateRoomDto room, CancellationToken cancellationToken = default);

    Task<DeactivateRoomResponse> DeactivateRoom(DeactivateRoomDto room, CancellationToken cancellationToken = default);

    Task<ReorderRoomsResponse> ReorderRooms(ReorderRoomsDto dto, CancellationToken cancellationToken = default);
}
