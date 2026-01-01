namespace Aro.UI.Application.DTOs;

public record ReorderPoliciesRequest(
    Guid PropertyId,
    List<ReorderPoliciesRequest.PolicyOrderItem> PolicyOrders
)
{
    public record PolicyOrderItem(
        Guid PolicyId,
        int DisplayOrder
    );
}
