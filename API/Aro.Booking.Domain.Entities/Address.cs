using Aro.Common.Domain.Entities;

namespace Aro.Booking.Domain.Entities;

public class Address : IEntity
{
    public Guid Id { get; set; }
    public string AddressLine1 { get; set; }
    public string AddressLine2 { get; set; }
    public string City { get; set; }
    public string PostalCode { get; set; }
    public string Country { get; set; }
    public string PhoneNumber { get; set; }
    public string Website { get; set; }

    public ICollection<Group> Groups { get; set; }
    public ICollection<Property> Properties { get; set; }
}
