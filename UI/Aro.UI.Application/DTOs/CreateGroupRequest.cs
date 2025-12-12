namespace Aro.UI.Application.DTOs;

public record CreateGroupRequest(
    string GroupName,
    string AddressLine1,
    string? AddressLine2,
    string City,
    string PostalCode,
    string Country,
    Guid PrimaryContactId,
    bool IsActive,
    byte[]? LogoBytes = null,
    string? LogoFileName = null,
    string? LogoContentType = null
);
