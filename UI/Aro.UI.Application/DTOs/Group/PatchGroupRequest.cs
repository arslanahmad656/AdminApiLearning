namespace Aro.UI.Application.DTOs.Group;

public record PatchGroupRequest(
    Guid Id,
    string? GroupName,
    string? AddressLine1,
    string? AddressLine2,
    string? City,
    string? Country,
    string? PostalCode,
    GroupLogo? Logo,
    Guid? PrimaryContactId,
    bool? IsActive
);
