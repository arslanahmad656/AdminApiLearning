using Aro.Admin.Application.Mediator.Group.Notifications;
using Aro.Admin.Application.Services.DataServices;
using Aro.Admin.Application.Services.DTOs.ServiceParameters.Audit;
using MediatR;

namespace Aro.Admin.Application.Mediator.Group.Handlers;

public class GroupCreatedNotificationHandler(IAuditService auditService) : INotificationHandler<GroupCreatedNotification>
{
    public async Task Handle(GroupCreatedNotification notification, CancellationToken cancellationToken)
    {
        await auditService.LogGroupCreated(
            new(
                notification.CreateGroupResponse.Id,
                notification.CreateGroupResponse.GroupName
            ), cancellationToken
        ).ConfigureAwait(false);
    }
}
