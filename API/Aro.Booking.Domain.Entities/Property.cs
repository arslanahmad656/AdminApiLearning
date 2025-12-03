using Aro.Booking.Domain.Shared;
using Aro.Common.Domain.Entities;

namespace Aro.Booking.Domain.Entities;

public class Property : IEntity
{
    public Guid Id { get; set; }
    public Guid? GroupId { get; set; }
    public string PropertyName { get; set; }
    public PropertyTypes PropertyTypes { get; set; }
    public int StarRating { get; set; }
    public string Currency { get; set; } // ISO currency code (e.g., "USD", "EUR", "GBP")
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;
    public Guid? AddressId { get; set; }
    public Guid? ContactId { get; set; }
    public Group Group { get; set; }
    public Address Address { get; set; }
    public Contact Contact { get; set; }
    public ICollection<Room> Rooms { get; set; }
}
