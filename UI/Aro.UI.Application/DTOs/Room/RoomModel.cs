namespace Aro.UI.Application.DTOs.Room;

public class RoomModel
{
    public Guid Id { get; set; } = Guid.Empty;

    public Guid PropertyId { get; set; } = Guid.Empty;

    public string RoomName { get; set; } = string.Empty;

    public string RoomCode { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int MaxOccupancy { get; set; } = 0;

    public int MaxAdults { get; set; } = 0;

    public int MaxChildren { get; set; } = 0;

    public int RoomSizeSQM { get; set; } = 0;

    public int RoomSizeSQFT { get; set; } = 0;


    public BedConfiguration BedConfig { get; set; } = BedConfiguration.Single;

    public List<Amenity>? Amenities { get; set; } = [];

    public List<ImageModel>? Images { get; set; } = [];

    public bool IsActive { get; set; } = true;

    public bool IsEditMode { get; set; } = false;
}