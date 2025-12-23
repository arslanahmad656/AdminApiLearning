namespace Aro.Booking.Application.Services.Group;

public record PrimaryContact(
    string Email,
    string Name,
    string CountryCode,
    string PhoneNumber
);