namespace Aro.Booking.Application.Services.Room;

public record CreateRoomDto(
    string RoomName,
    string RoomCode,
    string? Description,
    int MaxOccupancy,
    int MaxAdults,
    int MaxChildren,
    int? RoomSizeSQM,
    int? RoomSizeSQFT,
    BedConfiguration BedConfig,
    List<Guid>? AmenityIds,
    bool IsActive
);

