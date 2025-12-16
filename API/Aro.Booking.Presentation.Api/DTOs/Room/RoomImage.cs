namespace Aro.Booking.Presentation.Api.DTOs.Room;

public record RoomImage(
    string Name,
    string ContentBase64,
    int OrderIndex,
    bool IsThumbnail
);