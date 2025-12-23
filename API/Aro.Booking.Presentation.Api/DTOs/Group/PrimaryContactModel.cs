namespace Aro.Booking.Presentation.Api.DTOs.Group;

public record PrimaryContactModel(
    string Email,
    string Name,
    string CountryCode,
    string PhoneNumber
);