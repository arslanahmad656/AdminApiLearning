using Aro.Booking.Application.Mediator.Policy.Queries;
using Aro.Booking.Application.Services.Policy;
using MediatR;

namespace Aro.Booking.Application.Mediator.Policy.Handlers;

public class GetPolicyCommandHandler(IPolicyService policyService) : IRequestHandler<GetPolicyQuery, DTOs.GetPolicyResponse>
{
    public async Task<DTOs.GetPolicyResponse> Handle(GetPolicyQuery request, CancellationToken cancellationToken)
    {
        var req = request.Data;
        var res = await policyService.GetPolicyById(
            new(
                req.Id,
                req.Include
            ), cancellationToken).ConfigureAwait(false);

        var policy = res.Policy;
        var result = new DTOs.GetPolicyResponse(
            new DTOs.PolicyDto(
                policy.Id,
                policy.Title,
                policy.Description,
                policy.IsActive
            ));

        return result;
    }
}

