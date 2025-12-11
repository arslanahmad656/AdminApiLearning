using Microsoft.AspNetCore.Http;

namespace Aro.Booking.Presentation.Api.DTOs;

public record CreateGroupModel(
    string GroupName,
    string AddressLine1,
    string? AddressLine2,
    string City,
    string PostalCode,
    string Country,
    IFormFile Logo,
    Guid PrimaryContactId,
    bool IsActive
);

