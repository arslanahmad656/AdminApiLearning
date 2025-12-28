namespace Aro.Booking.Application.Mediator.Room.DTOs;

public record DeactivateRoomResponse(
    Guid Id,
    bool IsActive
);

