namespace Aro.Booking.Application.Mediator.Room.DTOs;

public record CreateRoomResponse(
    Guid Id,
    string? RoomName
);

