namespace Aro.Booking.Application.Mediator.Room.DTOs;

public record CreateRoomRequest(
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
