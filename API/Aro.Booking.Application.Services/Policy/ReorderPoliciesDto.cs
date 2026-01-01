namespace Aro.Booking.Application.Services.Policy;

public record ReorderPoliciesDto(
    Guid PropertyId,
    List<ReorderPoliciesDto.PolicyOrderItem> PolicyOrders
)
{
    public record PolicyOrderItem(
        Guid PolicyId,
        int DisplayOrder
    );
}
