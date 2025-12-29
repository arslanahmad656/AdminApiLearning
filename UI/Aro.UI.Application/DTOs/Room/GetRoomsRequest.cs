namespace Aro.UI.Application.DTOs.Room;
public record GetRoomsRequest(
    Guid? PropertyId,
    string? Filter,
    string? Include,
    int Page,
    int PageSize,
    string SortBy,
    bool Ascending
);

