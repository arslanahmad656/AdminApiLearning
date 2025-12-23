namespace Aro.Booking.Presentation.Api.DTOs.Group;

public record PatchGroupModel(
    string? GroupName,
    string? AddressLine1,
    string? AddressLine2,
    string? City,
    string? PostalCode,
    string? Country,
    GroupLogo? Logo,
    Guid? PrimaryContactId,
    bool? IsActive
);

