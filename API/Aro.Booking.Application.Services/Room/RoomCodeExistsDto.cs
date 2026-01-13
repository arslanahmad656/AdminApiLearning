namespace Aro.Booking.Application.Services.Room;

public record RoomCodeExistsDto(
    Guid PropertyId,
    string RoomCode
);

