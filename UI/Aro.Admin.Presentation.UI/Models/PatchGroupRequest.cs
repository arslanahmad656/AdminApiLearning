namespace Aro.Admin.Presentation.UI.Models;

public record PatchGroupRequest(
    Guid Id,
    string? GroupName,
    string? AddressLine1,
    string? AddressLine2,
    string? City,
    string? PostalCode,
    string? Country,
    Guid? PrimaryContactId
);
