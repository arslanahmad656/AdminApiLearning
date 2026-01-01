namespace Aro.Booking.Presentation.Api.DTOs;

public record ReorderPoliciesModel(
    Guid PropertyId,
    List<ReorderPoliciesModel.PolicyOrderItemModel> PolicyOrders
)
{
    public record PolicyOrderItemModel(
        Guid PolicyId,
        int DisplayOrder
    );
}
