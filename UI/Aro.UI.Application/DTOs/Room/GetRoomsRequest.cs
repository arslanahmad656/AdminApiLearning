namespace Aro.UI.Application.DTOs.Room;
public record GetRoomsRequest(
    string Filter,
    string Include,
    int Page,
    int PageSize,
    string SortBy,
    bool Ascending
);

