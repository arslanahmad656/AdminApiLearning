namespace Aro.Admin.Presentation.Api.DTOs;

public record GetGroupsQueryParameters(
    string? Include = null,
    int Page = 1,
    int PageSize = 20,
    string SortBy = "GroupName",
    bool Ascending = true
);

