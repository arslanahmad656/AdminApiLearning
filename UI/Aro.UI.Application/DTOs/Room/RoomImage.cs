namespace Aro.UI.Application.DTOs.Room;

public record RoomImage(
    string Name,
    string ContentBase64,
    int OrderIndex,
    bool IsThumbnail
);
