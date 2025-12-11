using Aro.Booking.Application.Mediator.Policy.Commands;
using Aro.Booking.Application.Mediator.Policy.Notifications;
using Aro.Booking.Application.Services.Policy;
using MediatR;

namespace Aro.Booking.Application.Mediator.Policy.Handlers;

public class PatchPolicyCommandHandler(IPolicyService policyService, IMediator mediator) : IRequestHandler<PatchPolicyCommand, DTOs.PatchPolicyResponse>
{
    public async Task<DTOs.PatchPolicyResponse> Handle(PatchPolicyCommand request, CancellationToken cancellationToken)
    {
        var req = request.PatchPolicyRequest.Policy;
        var res = await policyService.PatchPolicy(new(new(
            req.Id,
            req.Title,
            req.Description,
            req.IsActive
        )), cancellationToken
        ).ConfigureAwait(false);

        var r = res.Policy;
        var result = new DTOs.PatchPolicyResponse(new(
            r.Id,
            r.PropertyId,
            r.Title,
            r.Description,
            r.IsActive
        ));

        await mediator.Publish(new PolicyPatchedNotification(result), cancellationToken).ConfigureAwait(false);

        return result;
    }
}

