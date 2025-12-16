namespace Aro.Booking.Presentation.Api.DTOs.Room;

public record CreateRoomModel(
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