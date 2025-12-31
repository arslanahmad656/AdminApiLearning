namespace Aro.Booking.Application.Mediator.Room.DTOs;

public record ReorderRoomsResponse(
    bool Success,
    int UpdatedCount
);
