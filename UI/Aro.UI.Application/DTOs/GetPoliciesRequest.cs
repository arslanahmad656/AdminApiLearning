namespace Aro.UI.Application.DTOs;

public record GetPoliciesRequest(
    char? Filter = null,
    string? Include = null,
    int Page = 1,
    int PageSize = 100,
    string SortBy = "Title",
    bool Ascending = true
);
