namespace Aro.Booking.Application.Services.Room;

public record ReorderRoomsDto(
    Guid PropertyId,
    List<ReorderRoomsDto.RoomOrderItem> RoomOrders
)
{
    public record RoomOrderItem(
        Guid RoomId,
        int DisplayOrder
    );
}
