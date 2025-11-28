namespace Aro.Common.Domain.Entities;

public class ContactInfo : IEntity
{
    public Guid Id { get; set; }
    public User User { get; set; }
    public string CountryCode { get; set; }
    public string PhoneNumber { get; set; }
}

