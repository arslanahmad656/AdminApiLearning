namespace Aro.UI.Application.DTOs.Group;

public record CreateGroupRequest(
    string GroupName,
    string AddressLine1,
    string? AddressLine2,
    string City,
    string Country,
    string PostalCode,
    GroupLogo Logo,
    PrimaryContactDto PrimaryContact,
    bool IsActive
);
