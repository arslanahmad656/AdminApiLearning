using Aro.Common.Application.Shared;

namespace Aro.Booking.Application.Services.Policy;

public interface IPolicyService : IService
{
    Task<CreatePolicyResponse> CreatePolicy(CreatePolicyDto policy, CancellationToken cancellationToken = default);

    Task<GetPoliciesResponse> GetPolicies(GetPoliciesDto query, CancellationToken cancellationToken = default);

    Task<GetPolicyResponse> GetPolicyById(GetPolicyDto query, CancellationToken cancellationToken = default);

    Task<PatchPolicyResponse> PatchPolicy(PatchPolicyDto policy, CancellationToken cancellationToken = default);

    Task<DeletePolicyResponse> DeletePolicy(DeletePolicyDto policy, CancellationToken cancellationToken = default);
}

