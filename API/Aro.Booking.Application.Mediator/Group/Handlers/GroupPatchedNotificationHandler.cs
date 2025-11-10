using Aro.Booking.Application.Mediator.Group.Notifications;
using Aro.Common.Application.Services.Audit;
using Aro.Common.Domain.Shared;
using MediatR;

namespace Aro.Booking.Application.Mediator.Group.Handlers;

public class GroupPatchedNotificationHandler(IAuditService auditService, AuditActions auditActions, EntityTypes entityTypes) : INotificationHandler<GroupPatchedNotification>
{
    public async Task Handle(GroupPatchedNotification notification, CancellationToken cancellationToken)
    {
        var log = new
        {
            notification.PatchGroupResponse.Id,
            notification.PatchGroupResponse.GroupName
        };

        await auditService.Log(
            new(
                auditActions.GroupPatched,
                entityTypes.Group,
                notification.PatchGroupResponse.Id.ToString(),
                log
            ), cancellationToken
        ).ConfigureAwait(false);
    }
}
