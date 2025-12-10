using Aro.Booking.Application.Mediator.Policy.Commands;
using Aro.Booking.Application.Mediator.Policy.Notifications;
using Aro.Booking.Application.Services.Policy;
using MediatR;

namespace Aro.Booking.Application.Mediator.Policy.Handlers;

public class DeletePolicyCommandHandler(IPolicyService policyService, IMediator mediator) : IRequestHandler<DeletePolicyCommand, DTOs.DeletePolicyResponse>
{
    public async Task<DTOs.DeletePolicyResponse> Handle(DeletePolicyCommand request, CancellationToken cancellationToken)
    {
        var req = request.DeletePolicyRequest;
        var res = await policyService.DeletePolicy(new(req.Id), cancellationToken).ConfigureAwait(false);

        var result = new DTOs.DeletePolicyResponse(res.Id);
        await mediator.Publish(new PolicyDeletedNotification(result), cancellationToken).ConfigureAwait(false);

        return result;
    }
}

