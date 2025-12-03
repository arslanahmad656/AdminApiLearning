namespace Aro.Booking.Application.Mediator.Room.DTOs;

public record GetRoomsRequest(
    char? Filter,
    string? Include,
    int Page,
    int PageSize,
    string SortBy,
    bool Ascending
);

