namespace Aro.UI.Application.DTOs.Room;

public record ReorderRoomsRequest(
    Guid PropertyId,
    List<ReorderRoomsRequest.RoomOrderItem> RoomOrders
)
{
    public record RoomOrderItem(
        Guid RoomId,
        int DisplayOrder
    );
}
