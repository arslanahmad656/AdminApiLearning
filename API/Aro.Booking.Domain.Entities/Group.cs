using Aro.Common.Domain.Entities;

namespace Aro.Booking.Domain.Entities;

public class Group : IEntity
{
    public Guid Id { get; set; }
    public string GroupName { get; set; }
    public string AddressLine1 { get; set; }
    public string AddressLine2 { get; set; }
    public string City { get; set; }
    public string PostalCode { get; set; }
    public string Country { get; set; }
    public byte[] Logo { get; set; }
    public Guid PrimaryContactId { get; set; }
    public bool IsActive { get; set; }
    public User PrimaryContact { get; set; }
}
