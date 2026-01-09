namespace Aro.Booking.Presentation.Api.DTOs.Group;

public record CreateGroupModel(
    string GroupName,
    string AddressLine1,
    string? AddressLine2,
    string City,
    string Country,
    string PostalCode,
    GroupLogo? Logo,
    PrimaryContactModel PrimaryContact,
    bool IsActive
);

