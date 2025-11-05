namespace Aro.Admin.Presentation.Api.DTOs;

public record PatchGroupModel(
    string? GroupName,
    string? AddressLine1,
    string? AddressLine2,
    string? City,
    string? PostalCode,
    string? Country,
    byte[]? Logo,
    Guid? PrimaryContactId,
    bool? IsActive
);

