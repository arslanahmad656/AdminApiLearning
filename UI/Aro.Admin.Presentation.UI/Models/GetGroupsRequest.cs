namespace Aro.Admin.Presentation.UI.Models;
public record GetGroupsRequest(
    char Filter,
    string Include,
    int Page,
    int PageSize,
    string SortBy,
    bool Ascending
);

