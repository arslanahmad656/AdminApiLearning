namespace Aro.Booking.Application.Mediator.Policy.DTOs;

public record ReorderPoliciesRequest(
    Guid PropertyId,
    List<ReorderPoliciesRequest.PolicyOrderItemDto> PolicyOrders
)
{
    public record PolicyOrderItemDto(
        Guid PolicyId,
        int DisplayOrder
    );
}
