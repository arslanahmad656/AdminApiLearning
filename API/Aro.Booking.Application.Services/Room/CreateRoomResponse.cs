namespace Aro.Booking.Application.Services.Room;

public record CreateRoomResponse(
    Guid Id,
    string? RoomName
);
