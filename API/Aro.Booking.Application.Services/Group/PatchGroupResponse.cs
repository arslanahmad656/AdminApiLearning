namespace Aro.Booking.Application.Services.Group;

public record PatchGroupResponse(
    Guid Id,
    string? GroupName,
    string? AddressLine1,
    string? AddressLine2,
    string? City,
    string? PostalCode,
    string? Country,
    Guid? LogoId,
    Guid? PrimaryContactId,
    bool? IsActive
);
