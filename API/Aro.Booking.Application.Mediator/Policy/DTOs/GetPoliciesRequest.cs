namespace Aro.Booking.Application.Mediator.Policy.DTOs;

public record GetPoliciesRequest(
    char? Filter,
    string? Include,
    int Page,
    int PageSize,
    string SortBy,
    bool Ascending
);

