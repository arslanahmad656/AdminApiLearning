using System.Collections.Generic;
using Aro.Common.Domain.Entities;

namespace Aro.Booking.Domain.Entities;

public class Group : IEntity
{
    public Guid Id { get; set; }
    public string GroupName { get; set; }
    //public byte[] Logo { get; set; }
    public Guid? IconId { get; set; }
    public bool IsActive { get; set; }
    //public Guid ContactId { get; set; }
    public Guid PrimaryContactId { get; set; }
    public Guid AddressId { get; set; }
    //public Contact Contact { get; set; }
    public Address Address { get; set; }
    public User PrimaryContact { get; set; }
    public FileResource Icon { get; set; }

    public ICollection<Property> Properties { get; set; }
}
