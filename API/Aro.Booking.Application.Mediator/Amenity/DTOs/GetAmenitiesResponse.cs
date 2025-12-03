namespace Aro.Booking.Application.Mediator.Amenity.DTOs;

public record GetAmenitiesResponse(
    List<AmenityDto> Amenities,
    int TotalCount
);

