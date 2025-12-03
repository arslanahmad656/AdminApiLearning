namespace Aro.Booking.Presentation.Api.DTOs;

public record GetAmenitiesQueryParameters(
    char? Filter = null,
    string? Include = null,
    int Page = 1,
    int PageSize = 20,
    string SortBy = "Name",
    bool Ascending = true
);

