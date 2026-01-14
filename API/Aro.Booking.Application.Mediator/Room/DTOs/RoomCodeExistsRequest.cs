namespace Aro.Booking.Application.Mediator.Room.DTOs;

public record RoomCodeExistsRequest(
    Guid PropertyId,
    string RoomCode
);

