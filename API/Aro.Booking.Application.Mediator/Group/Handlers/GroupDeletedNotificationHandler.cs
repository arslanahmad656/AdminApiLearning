using Aro.Booking.Application.Mediator.Group.Notifications;
using Aro.Common.Application.Services.Audit;
using Aro.Common.Domain.Shared;
using MediatR;

namespace Aro.Booking.Application.Mediator.Group.Handlers;

public class GroupDeletedNotificationHandler(IAuditService auditService, AuditActions auditActions, EntityTypes entityTypes) : INotificationHandler<GroupDeletedNotification>
{
    public async Task Handle(GroupDeletedNotification notification, CancellationToken cancellationToken)
    {
        var log = new
        {
            notification.DeleteGroupResponse.Id
        };

        await auditService.Log(
            new(
                auditActions.GroupDeleted,
                entityTypes.Group,
                notification.DeleteGroupResponse.Id.ToString(),
                log
            ), cancellationToken
        ).ConfigureAwait(false);
    }
}
