namespace Aro.UI.Application.DTOs;

public record GetPoliciesResponse(
    List<PolicyDto> Policies,
    int TotalCount
);
