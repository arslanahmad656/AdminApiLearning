namespace Aro.Booking.Application.Services.Group;

public record GetGroupsDto(
    string Include,
    int Page,
    int PageSize,
    string SortBy,
    bool Ascending
);

