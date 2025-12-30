namespace Aro.Booking.Application.Mediator.Room.DTOs;

public record RoomDto(
    Guid Id,
    string RoomName,
    string RoomCode,
    string? Description,
    int MaxOccupancy,
    int MaxAdults,
    int MaxChildren,
    int? RoomSizeSQM,
    BedConfiguration BedConfig,
    List<Guid>? AmenityIds,
    bool IsActive,
    List<RoomImageInfoDto>? Images = null
);

public record RoomImageInfoDto(
    Guid FileId,
    int OrderIndex,
    bool IsThumbnail
);