namespace Aro.Booking.Application.Services.Room;

public record CreateRoomDto(
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

