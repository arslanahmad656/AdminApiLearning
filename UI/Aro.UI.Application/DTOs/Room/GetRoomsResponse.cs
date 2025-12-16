namespace Aro.UI.Application.DTOs.Room;
public record GetRoomsResponse(
    List<RoomDto> Rooms,
    int TotalCount
);

