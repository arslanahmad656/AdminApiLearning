namespace Aro.Booking.Application.Services.Policy;

public record ReorderPoliciesResponse(
    bool Success,
    int UpdatedCount
);
