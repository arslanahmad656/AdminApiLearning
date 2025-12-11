using Aro.Booking.Application.Mediator.Policy.Commands;
using Aro.Booking.Application.Mediator.Policy.Notifications;
using Aro.Booking.Application.Services.Policy;
using MediatR;

namespace Aro.Booking.Application.Mediator.Policy.Handlers;

public class CreatePolicyCommandHandler(IPolicyService policyService, IMediator mediator) : IRequestHandler<CreatePolicyCommand, DTOs.CreatePolicyResponse>
{
    public async Task<DTOs.CreatePolicyResponse> Handle(CreatePolicyCommand request, CancellationToken cancellationToken)
    {
        var r = request.CreatePolicyRequest;
        var response = await policyService.CreatePolicy(
            new CreatePolicyDto(
                r.Title,
                r.Description,
                r.IsActive
            ), cancellationToken
        ).ConfigureAwait(false);

        var result = new DTOs.CreatePolicyResponse(response.Id, response.Title);
        await mediator.Publish(new PolicyCreatedNotification(result), cancellationToken).ConfigureAwait(false);

        return result;
    }
}

