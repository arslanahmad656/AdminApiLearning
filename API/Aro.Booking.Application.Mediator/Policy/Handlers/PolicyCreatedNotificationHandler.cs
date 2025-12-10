using Aro.Booking.Application.Mediator.Policy.Notifications;
using Aro.Common.Application.Services.Audit;
using Aro.Common.Domain.Shared;
using MediatR;

namespace Aro.Booking.Application.Mediator.Policy.Handlers;

public class PolicyCreatedNotificationHandler(IAuditService auditService, AuditActions auditActions, EntityTypes entityTypes) : INotificationHandler<PolicyCreatedNotification>
{
    public async Task Handle(PolicyCreatedNotification notification, CancellationToken cancellationToken)
    {
        var log = new
        {
            notification.CreatePolicyResponse.Id,
            notification.CreatePolicyResponse.Title
        };

        await auditService.Log(
            new(
                auditActions.PolicyCreated,
                entityTypes.Policy,
                notification.CreatePolicyResponse.Id.ToString(),
                log
            ), cancellationToken
        ).ConfigureAwait(false);
    }
}

