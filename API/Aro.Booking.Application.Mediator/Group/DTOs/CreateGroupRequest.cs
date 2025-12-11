namespace Aro.Booking.Application.Mediator.Group.DTOs;

public record CreateGroupRequest(
    string GroupName,
    string AddressLine1,
    string? AddressLine2,
    string City,
    string PostalCode,
    string Country,
    Stream Logo,
    Guid PrimaryContactId,
    bool IsActive
);
