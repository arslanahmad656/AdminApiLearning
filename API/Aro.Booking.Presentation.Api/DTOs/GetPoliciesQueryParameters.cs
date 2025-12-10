namespace Aro.Booking.Presentation.Api.DTOs;

public record GetPoliciesQueryParameters(
    char? Filter,
    string? Include,
    int Page = 1,
    int PageSize = 10,
    string SortBy = "Title",
    bool Ascending = true
);

