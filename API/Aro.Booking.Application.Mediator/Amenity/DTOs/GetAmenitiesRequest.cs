namespace Aro.Booking.Application.Mediator.Amenity.DTOs;

public record GetAmenitiesRequest(
    char? Filter,
    string? Include,
    int Page,
    int PageSize,
    string SortBy,
    bool Ascending
);

