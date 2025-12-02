namespace Aro.Booking.Application.Services.Room;

public record GetRoomsResponse(
    List<RoomDto> Rooms,
    int TotalCount
);

