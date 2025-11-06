namespace Aro.Booking.Application.Mediator.Group.DTOs;

public record GetGroupsRequest(
    string? Include,
    int Page,
    int PageSize,
    string SortBy,
    bool Ascending
);

