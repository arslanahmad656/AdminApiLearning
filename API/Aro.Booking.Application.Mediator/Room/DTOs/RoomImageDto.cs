namespace Aro.Booking.Application.Mediator.Room.DTOs;

public record RoomImageDto(
    string Name,
    Stream Content,
    int OrderIndex,
    bool IsThumbnail
);