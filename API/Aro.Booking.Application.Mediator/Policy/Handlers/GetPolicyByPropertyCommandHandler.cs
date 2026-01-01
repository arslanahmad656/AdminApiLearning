using Aro.Booking.Application.Mediator.Policy.Queries;
using Aro.Booking.Application.Services.Policy;
using MediatR;

namespace Aro.Booking.Application.Mediator.Policy.Handlers;

public class GetPolicyByPropertyCommandHandler(IPolicyService policyService) : IRequestHandler<GetPolicyByPropertyQuery, DTOs.GetPolicyResponse>
{
    public async Task<DTOs.GetPolicyResponse> Handle(GetPolicyByPropertyQuery request, CancellationToken cancellationToken)
    {
        var req = request.Data;
        var res = await policyService.GetPolicyByProperty(
            new(
                req.PropertyId,
                req.PolicyId,
                req.Include
            ), cancellationToken).ConfigureAwait(false);

        var policy = res.Policy;
        var result = new DTOs.GetPolicyResponse(
            new DTOs.PolicyDto(
                policy.Id,
                policy.PropertyId,
                policy.Title,
                policy.Description,
                policy.IsActive,
                policy.DisplayOrder
            ));

        return result;
    }
}

