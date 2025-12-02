namespace Aro.Booking.Presentation.Api.DTOs;

public record PatchRoomModel(
    string? RoomName,
    string? RoomCode,
    string? Description,
    int? MaxOccupancy,
    int? MaxAdults,
    int? MaxChildren,
    int? RoomSizeSQM,
    int? RoomSizeSQFT,
    BedConfiguration? BedConfig,
    List<Guid>? AmenityIds,
    bool? IsActive
);

