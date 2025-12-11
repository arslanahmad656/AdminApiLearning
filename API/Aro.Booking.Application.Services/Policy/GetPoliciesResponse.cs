namespace Aro.Booking.Application.Services.Policy;

public record GetPoliciesResponse(
    List<PolicyDto> Policies,
    int TotalCount
);

