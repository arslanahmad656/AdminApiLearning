namespace Aro.Booking.Application.Services.Room;

public record DeactivateRoomResponse(
    Guid Id,
    bool IsActive
);

