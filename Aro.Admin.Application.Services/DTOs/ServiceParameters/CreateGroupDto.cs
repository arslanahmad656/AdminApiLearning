namespace Aro.Admin.Application.Services.DTOs.ServiceParameters;

public record CreateGroupDto(
    string GroupName,
    string AddressLine1,
    string? AddressLine2,
    string City,
    string PostalCode,
    string Country,
    byte[]? Logo,
    Guid PrimaryContactId,
    bool IsActive
);

