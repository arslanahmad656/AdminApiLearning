using Aro.Booking.Application.Mediator.Policy.Queries;
using Aro.Booking.Application.Services.Policy;
using MediatR;

namespace Aro.Booking.Application.Mediator.Policy.Handlers;

public class GetPoliciesCommandHandler(IPolicyService policyService) : IRequestHandler<GetPoliciesQuery, DTOs.GetPoliciesResponse>
{
    public async Task<DTOs.GetPoliciesResponse> Handle(GetPoliciesQuery request, CancellationToken cancellationToken)
    {
        var req = request.Data;
        var res = await policyService.GetPolicies(
            new(
                req.Filter,
                req.Include,
                req.Page,
                req.PageSize,
                req.SortBy,
                req.Ascending
            ), cancellationToken).ConfigureAwait(false);

        var policyDtos = res.Policies
            .Select(p => new DTOs.PolicyDto(
                p.Id,
                p.Title,
                p.Description,
                p.IsActive
            ))
            .ToList();

        return new DTOs.GetPoliciesResponse(policyDtos, res.TotalCount);
    }
}

