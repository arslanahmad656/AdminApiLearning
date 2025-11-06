using Aro.Booking.Application.Mediator.Group.Notifications;
using Aro.Common.Application.Services.Audit;
using Aro.Common.Domain.Shared;
using MediatR;

namespace Aro.Booking.Application.Mediator.Group.Handlers;

public class GroupCreatedNotificationHandler(IAuditService auditService, AuditActions auditActions, EntityTypes entityTypes) : INotificationHandler<GroupCreatedNotification>
{
    public async Task Handle(GroupCreatedNotification notification, CancellationToken cancellationToken)
    {
        var log = new
        {
            notification.CreateGroupResponse.Id,
            notification.CreateGroupResponse.GroupName
        };

        await auditService.Log(
            new(
                auditActions.GroupCreated,
                entityTypes.Group,
                notification.CreateGroupResponse.Id.ToString(),
                log
            ), cancellationToken
        ).ConfigureAwait(false);
    }
}
