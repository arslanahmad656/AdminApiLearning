using Aro.UI.Application.DTOs.Room;

namespace Aro.UI.Infrastructure.Services;

public interface IRoomService
{
    Task<CreateRoomResponse?> CreateRoom(CreateRoomRequest request);

    Task<GetRoomResponse?> GetRoom(GetRoomRequest request);

    Task<GetRoomsResponse?> GetRooms(GetRoomsRequest request);

    Task<bool> RoomCodeExists(GetRoomByRoomCodeRequest request);

    Task<PatchRoomResponse?> PatchRoom(PatchRoomRequest request);

    Task<DeleteRoomResponse?> DeleteRoom(Guid Id);

    Task<bool> ActivateRoom(Guid roomId);

    Task<bool> DeactivateRoom(Guid roomId);

    Task<bool> ReorderRooms(ReorderRoomsRequest request);

    Task<byte[]?> GetRoomImage(Guid roomId, Guid imageId);
}
