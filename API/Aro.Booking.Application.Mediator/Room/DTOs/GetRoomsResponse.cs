namespace Aro.Booking.Application.Mediator.Room.DTOs;

public record GetRoomsResponse(
    List<RoomDto> Rooms,
    int TotalCount
);

