namespace Aro.Booking.Application.Services.Room;

public record GetRoomsDto(
    Guid? PropertyId,
    char? Filter,
    string? Include,
    int Page,
    int PageSize,
    string SortBy,
    bool Ascending
);

