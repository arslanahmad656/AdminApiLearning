namespace Aro.Common.Domain.Entities;

public class Country : IEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string OfficialName { get; set; }
    public string ISO2 { get; set; }
    public string PostalCodeRegex { get; set; }
    public string PhoneCountryCode { get; set; }
    public string PhoneNumberRegex { get; set; }
}