namespace Aro.Booking.Application.Mediator.Policy.DTOs;

public record GetPoliciesResponse(
    List<PolicyDto> Policies,
    int TotalCount
);

