namespace Aro.UI.Application.DTOs.Room;

// TEST CLASS
//public class RoomModel
//{
//    public Guid Id { get; set; } = Guid.Empty;

//    public Guid PropertyId { get; set; }

//    public string RoomName { get; set; } = "Jack's room";

//    public string RoomCode { get; set; } = "JACK";

//    public string Description { get; set; } = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Praesent id scelerisque dolor. Aliquam mattis, urna vitae maximus porta, ligula arcu posuere enim, sagittis egestas tellus tellus sed libero. Etiam vehicula sapien eu augue congue tempor. Suspendisse sapien arcu, egestas eget posuere sed, tincidunt sit amet turpis. Mauris a sapien sed enim faucibus luctus. Suspendisse vitae turpis non justo consectetur porta eu sit amet felis. Sed blandit volutpat nibh. Aenean commodo enim arcu, eu facilisis eros scelerisque in. Morbi vitae neque vel dolor volutpat suscipit eget a lacus. Praesent nisi lorem, ultricies sit amet sem eget, fringilla egestas nunc. Ut rhoncus elementum nunc, a dignissim enim semper et. Nullam porttitor ligula diam, id gravida nibh porta id.";

//    public int MaxOccupancy { get; set; } = 2;

//    public int MaxAdults { get; set; } = 1;

//    public int MaxChildren { get; set; } = 1;

//    public int RoomSizeSQM { get; set; } = 100;

//    public int RoomSizeSQFT { get; set; } = 100;


//    public BedConfiguration BedConfig = BedConfiguration.Single;

//    public List<Amenity> Amenities { get; set; } =
//    [
//        new Amenity { Name = "Free WiFi" },
//        new Amenity { Name = "Air Conditioning" },
//        new Amenity { Name = "Flat Screen TV" }
//    ];

//    public List<ImageModel>? Images { get; set; } = [];

//    public bool IsActive { get; set; } = true;

//    public bool IsEditMode { get; set; } = false;
//}

// DEFAULT CLASS

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


    public BedConfiguration BedConfig = BedConfiguration.Single;

    public List<Amenity>? Amenities { get; set; } = [];

    public List<ImageModel>? Images { get; set; } = [];

    public bool IsActive { get; set; } = true;

    public bool IsEditMode { get; set; } = false;
}