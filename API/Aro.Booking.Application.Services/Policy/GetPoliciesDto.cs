namespace Aro.Booking.Application.Services.Policy;

public record GetPoliciesDto(
    char? Filter,
    string? Include,
    int Page,
    int PageSize,
    string SortBy,
    bool Ascending
);

