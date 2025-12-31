namespace Aro.Booking.Presentation.Api.DTOs.Room;

public record ReorderRoomsModel(
    Guid PropertyId,
    List<ReorderRoomsModel.RoomOrderItemModel> RoomOrders
)
{
    public record RoomOrderItemModel(
        Guid RoomId,
        int DisplayOrder
    );
}
