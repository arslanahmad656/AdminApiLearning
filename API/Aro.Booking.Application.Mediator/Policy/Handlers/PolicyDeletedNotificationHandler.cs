using Aro.Booking.Application.Mediator.Policy.Notifications;
using Aro.Common.Application.Services.Audit;
using Aro.Common.Domain.Shared;
using MediatR;

namespace Aro.Booking.Application.Mediator.Policy.Handlers;

public class PolicyDeletedNotificationHandler(IAuditService auditService, AuditActions auditActions, EntityTypes entityTypes) : INotificationHandler<PolicyDeletedNotification>
{
    public async Task Handle(PolicyDeletedNotification notification, CancellationToken cancellationToken)
    {
        var log = new
        {
            notification.DeletePolicyResponse.Id
        };

        await auditService.Log(
            new(
                auditActions.PolicyDeleted,
                entityTypes.Policy,
                notification.DeletePolicyResponse.Id.ToString(),
                log
            ), cancellationToken
        ).ConfigureAwait(false);
    }
}

