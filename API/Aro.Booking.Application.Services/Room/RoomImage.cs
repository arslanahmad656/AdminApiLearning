namespace Aro.Booking.Application.Services.Room;

public record RoomImage(
    string Name,
    Stream Content,
    int OrderIndex,
    bool IsThumbnail
);

