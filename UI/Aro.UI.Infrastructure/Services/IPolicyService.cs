using Aro.UI.Application.DTOs;

namespace Aro.UI.Infrastructure.Services;

public interface IPolicyService
{
    Task<CreatePolicyResponse?> CreatePolicy(CreatePolicyRequest request);
    Task<GetPolicyResponse?> GetPolicy(GetPolicyRequest request);
    Task<GetPoliciesResponse?> GetPolicies(GetPoliciesRequest request);
    Task<PatchPolicyResponse?> PatchPolicy(PatchPolicyRequest request);
    Task<DeletePolicyResponse?> DeletePolicy(Guid id);
}
