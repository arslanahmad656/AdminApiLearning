namespace Aro.Booking.Application.Mediator.Group.DTOs;

public record PrimaryContactDto(
    string Email,
    string Name,
    string CountryCode,
    string PhoneNumber
);
