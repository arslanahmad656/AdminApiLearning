namespace Aro.UI.Application.DTOs;
public record GetGroupsRequest(
    string Filter,
    string Include,
    int Page,
    int PageSize,
    string SortBy,
    bool Ascending
);

