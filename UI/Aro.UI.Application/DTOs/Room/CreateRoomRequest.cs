namespace Aro.UI.Application.DTOs.Room;

public record CreateRoomRequest(
    Guid PropertyId,
    string RoomName,
    string RoomCode,
    string? Description,
    int MaxOccupancy,
    int MaxAdults,
    int MaxChildren,
    int RoomSizeSQM,
    BedConfiguration BedConfig,
    List<string>? Amenities,
    List<RoomImage>? RoomImages,
    bool IsActive
);
