namespace Aro.Booking.Application.Services.Amenity;

public record GetAmenitiesResponse(
    List<AmenityDto> Amenities,
    int TotalCount
);
