namespace Aro.Booking.Application.Mediator.Amenity.DTOs;

public record GetAmenityRequest(
    Guid Id,
    string? Include
);

