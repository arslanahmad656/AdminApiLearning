namespace Aro.Booking.Application.Mediator.Room.DTOs;

public record ReorderRoomsRequest(
    Guid PropertyId,
    List<RoomOrderItemDto> RoomOrders
);

public record RoomOrderItemDto(
    Guid RoomId,
    int DisplayOrder
);
