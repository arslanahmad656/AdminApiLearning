namespace Aro.Booking.Application.Services.Group;

public record PatchGroupDto(
    Guid Id,
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

