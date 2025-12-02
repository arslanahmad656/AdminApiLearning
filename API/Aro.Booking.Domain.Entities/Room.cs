using Aro.Common.Domain.Entities;

namespace Aro.Booking.Domain.Entities;

public class Room : IEntity
{
    public Guid Id { get; set; }
    public string RoomName { get; set; }
    public string RoomCode { get; set; }
    public string Description { get; set; }
    public int MaxOccupancy { get; set; }
    public int MaxAdults { get; set; }
    public int MaxChildren { get; set; }
    public int RoomSizeSQM { get; set; }
    public int RoomSizeSQFT { get; set; }
    public BedConfiguration BedConfig { get; set; }
    public bool IsActive { get; set; }
    public ICollection<RoomAmenity> RoomAmenities { get; set; } = [];
}
