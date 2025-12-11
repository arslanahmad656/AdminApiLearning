namespace Aro.Booking.Application.Services.Group;

public record CreateGroupDto(
    string GroupName,
    string AddressLine1,
    string? AddressLine2,
    string City,
    string PostalCode,
    string Country,
    Stream Logo,
    Guid ContactId,
    bool IsActive
);

