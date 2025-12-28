namespace Aro.Booking.Application.Mediator.Room.DTOs;

public record ActivateRoomResponse(
    Guid Id,
    bool IsActive
);

