namespace Aro.Booking.Presentation.Api.DTOs.Group;

public record GetGroupsQueryParameters(
    char? Filter = null,
    string? Include = null,
    int Page = 1,
    int PageSize = 20,
    string SortBy = "GroupName",
    bool Ascending = true
);

