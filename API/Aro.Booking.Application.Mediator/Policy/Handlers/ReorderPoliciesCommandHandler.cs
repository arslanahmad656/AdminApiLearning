using Aro.Booking.Application.Mediator.Policy.Commands;
using Aro.Booking.Application.Services.Policy;
using MediatR;
using MediatorDTOs = Aro.Booking.Application.Mediator.Policy.DTOs;
using ServiceDTOs = Aro.Booking.Application.Services.Policy;

namespace Aro.Booking.Application.Mediator.Policy.Handlers;

public class ReorderPoliciesCommandHandler(IPolicyService policyService) : IRequestHandler<ReorderPoliciesCommand, MediatorDTOs.ReorderPoliciesResponse>
{
    public async Task<MediatorDTOs.ReorderPoliciesResponse> Handle(ReorderPoliciesCommand request, CancellationToken cancellationToken)
    {
        var req = request.Request;
        var policyOrders = req.PolicyOrders.Select(po => new ServiceDTOs.ReorderPoliciesDto.PolicyOrderItem(po.PolicyId, po.DisplayOrder)).ToList();

        var res = await policyService.ReorderPolicies(
            new ServiceDTOs.ReorderPoliciesDto(
                req.PropertyId,
                policyOrders
            ), cancellationToken
        ).ConfigureAwait(false);

        return new MediatorDTOs.ReorderPoliciesResponse(res.Success, res.UpdatedCount);
    }
}
