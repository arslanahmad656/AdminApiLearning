namespace Aro.Booking.Presentation.Api.DTOs.Room;

public record GetRoomsQueryParameters(
    Guid? PropertyId = null,
    char? Filter = null,
    string? Include = null,
    int Page = 1,
    int PageSize = 20,
    string SortBy = "RoomName",
    bool Ascending = true
);

