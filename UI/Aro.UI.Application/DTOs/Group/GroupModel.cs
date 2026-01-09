namespace Aro.UI.Application.DTOs.Group;

public class GroupModel
{
    public Guid Id { get; set; } = Guid.Empty;

    // Default
    //public string GroupName { get; set; } = string.Empty;
    //public string? AddressLine1 { get; set; } = string.Empty;
    //public string? AddressLine2 { get; set; } = string.Empty;
    //public string? City { get; set; } = string.Empty;
    //public string? Country { get; set; } = string.Empty;
    //public string? PostalCode { get; set; } = string.Empty;

    // Testing
    public string GroupName { get; set; } = "Jack's Group";
    public string? AddressLine1 { get; set; } = "Address Line 1";
    public string? AddressLine2 { get; set; } = "Address Line 2";
    public string? City { get; set; } = "City";
    public string? Country { get; set; } = "United Kingdom";
    public string? PostalCode { get; set; } = "BT7 2FB";

    public ImageModel? Logo { get; set; }
    public required PrimaryContactModel PrimaryContact { get; set; }

    public GroupModel Clone()
    {
        return new GroupModel
        {
            Id = Id,
            GroupName = GroupName,
            AddressLine1 = AddressLine1,
            AddressLine2 = AddressLine2,
            City = City,
            Country = Country,
            PostalCode = PostalCode,
            Logo = Logo,
            PrimaryContact = PrimaryContact
        };
    }

    public bool? IsActive { get; set; } = true;
}