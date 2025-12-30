namespace Aro.UI.Application.DTOs.Room;

public record ReorderRoomsRequest(
    Guid PropertyId,
    List<RoomOrderItem> RoomOrders
);

public record RoomOrderItem(
    Guid RoomId,
    int DisplayOrder
);
