namespace Aro.UI.Application.DTOs.Room;

public record GetRoomByRoomCodeRequest(
    Guid PropertyId,
    string RoomCode
);

