namespace Aro.UI.Application.DTOs.Group;

public class PrimaryContactModel
{
    public Guid Id { get; set; } = Guid.Empty;

    // Default
    //public string Email { get; set; } = string.Empty;
    //public string Name { get; set; } = string.Empty;
    //public string CountryCode { get; set; } = string.Empty;
    //public string PhoneNumber { get; set; } = string.Empty;

    // Testing
    public string Email { get; set; } = "jack.coyle@aro.ie";
    public string Name { get; set; } = "Jack Coyle";
    public string CountryCode { get; set; } = "44";
    public string PhoneNumber { get; set; } = "750012345";
}