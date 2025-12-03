using Aro.Common.Domain.Entities;

namespace Aro.Booking.Domain.Entities
{
    public class Amenity : IEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public ICollection<RoomAmenity> RoomAmenities { get; set; } = [];
    }
}