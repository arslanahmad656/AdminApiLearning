namespace Aro.Booking.Application.Mediator.Room.DTOs;

public record GetRoomRequest(
    Guid Id,
    string? Include
);

