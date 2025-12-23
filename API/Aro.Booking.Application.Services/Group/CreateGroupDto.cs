namespace Aro.Booking.Application.Services.Group;

public record CreateGroupDto(
    string GroupName,
    string AddressLine1,
    string? AddressLine2,
    string City,
    string Country,
    string PostalCode,
    GroupLogo Logo,
    PrimaryContact PrimaryContact,
    bool IsActive
);