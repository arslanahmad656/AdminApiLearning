namespace Aro.Booking.Application.Services.Room;

public record ActivateRoomResponse(
    Guid Id,
    bool IsActive
);

