using Aro.UI.Application.DTOs.Room;

namespace Aro.UI.Infrastructure.Services;

public interface IRoomService
{
    Task<CreateRoomResponse?> CreateRoom(CreateRoomRequest request);

    Task<GetRoomResponse?> GetRoom(GetRoomRequest request);

    Task<GetRoomsResponse?> GetRooms(GetRoomsRequest request);

    Task<PatchRoomResponse> PatchRoom(PatchRoomRequest request);

    Task<DeleteRoomResponse?> DeleteRoom(Guid Id);
}
