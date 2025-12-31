namespace Aro.Booking.Application.Services.Room;

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
    int DisplayOrder,
    List<RoomDto.RoomImageInfoDto>? Images = null
)
{
    public record RoomImageInfoDto(
        Guid FileId,
        int OrderIndex,
        bool IsThumbnail
    );
}

