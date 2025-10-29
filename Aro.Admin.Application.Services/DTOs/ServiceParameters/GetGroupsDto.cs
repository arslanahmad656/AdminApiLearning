namespace Aro.Admin.Application.Services.DTOs.ServiceParameters;

public record GetGroupsDto(
    string Include,
    int Page,
    int PageSize,
    string SortBy,
    bool Ascending
);

