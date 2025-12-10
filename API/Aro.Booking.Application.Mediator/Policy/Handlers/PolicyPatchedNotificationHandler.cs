using Aro.Booking.Application.Mediator.Policy.Notifications;
using Aro.Common.Application.Services.Audit;
using Aro.Common.Domain.Shared;
using MediatR;

namespace Aro.Booking.Application.Mediator.Policy.Handlers;

public class PolicyPatchedNotificationHandler(IAuditService auditService, AuditActions auditActions, EntityTypes entityTypes) : INotificationHandler<PolicyPatchedNotification>
{
    public async Task Handle(PolicyPatchedNotification notification, CancellationToken cancellationToken)
    {
        var policy = notification.PatchPolicyResponse.Policy;
        var log = new
        {
            policy.Id,
            policy.Title,
            policy.Description,
            policy.IsActive
        };

        await auditService.Log(
            new(
                auditActions.PolicyPatched,
                entityTypes.Policy,
                policy.Id.ToString(),
                log
            ), cancellationToken
        ).ConfigureAwait(false);
    }
}

