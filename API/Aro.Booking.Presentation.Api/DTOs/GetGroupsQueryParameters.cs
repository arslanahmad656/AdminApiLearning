namespace Aro.Booking.Presentation.Api.DTOs;

public record GetGroupsQueryParameters(
    string? NameFilter = null,
    string? Include = null,
    int Page = 1,
    int PageSize = 20,
    string SortBy = "GroupName",
    bool Ascending = true
);

