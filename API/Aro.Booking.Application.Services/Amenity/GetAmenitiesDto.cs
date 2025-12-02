namespace Aro.Booking.Application.Services.Amenity;

public record GetAmenitiesDto(
    char? Filter,
    string? Include,
    int Page,
    int PageSize,
    string SortBy,
    bool Ascending
);

