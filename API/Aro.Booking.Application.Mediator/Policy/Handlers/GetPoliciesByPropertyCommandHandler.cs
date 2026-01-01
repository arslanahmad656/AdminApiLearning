using Aro.Booking.Application.Mediator.Policy.Queries;
using Aro.Booking.Application.Services.Policy;
using MediatR;

namespace Aro.Booking.Application.Mediator.Policy.Handlers;

public class GetPoliciesByPropertyCommandHandler(IPolicyService policyService) : IRequestHandler<GetPoliciesByPropertyQuery, DTOs.GetPoliciesResponse>
{
    public async Task<DTOs.GetPoliciesResponse> Handle(GetPoliciesByPropertyQuery request, CancellationToken cancellationToken)
    {
        var req = request.Data;
        var res = await policyService.GetPoliciesByProperty(
            new(
                req.PropertyId,
                req.Include
            ), cancellationToken).ConfigureAwait(false);

        var policyDtos = res.Policies
            .Select(p => new DTOs.PolicyDto(
                p.Id,
                p.PropertyId,
                p.Title,
                p.Description,
                p.IsActive,
                p.DisplayOrder
            ))
            .ToList();

        return new DTOs.GetPoliciesResponse(policyDtos, res.TotalCount);
    }
}

