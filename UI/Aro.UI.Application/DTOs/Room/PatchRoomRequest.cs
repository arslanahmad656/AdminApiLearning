namespace Aro.UI.Application.DTOs.Room;

public record PatchRoomRequest(
    Guid Id,
    string? RoomName,
    string? RoomCode,
    string? Description,
    int? MaxOccupancy,
    int? MaxAdults,
    int? MaxChildren,
    int? RoomSizeSQM,
    BedConfiguration? BedConfig,
    List<Amenity>? Amenities,
    List<ImageModel>? Images,
    Guid? ThumbnailId
);
