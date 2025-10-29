using Aro.Admin.Application.Mediator.Group.Notifications;
using Aro.Admin.Application.Services.DataServices;
using Aro.Admin.Application.Services.DTOs.ServiceParameters.Audit;
using MediatR;

namespace Aro.Admin.Application.Mediator.Group.Handlers;

public class GroupDeletedNotificationHandler(IAuditService auditService) : INotificationHandler<GroupDeletedNotification>
{
    public async Task Handle(GroupDeletedNotification notification, CancellationToken cancellationToken)
    {
        await auditService.LogGroupDeleted(
            new(
                notification.DeleteGroupResponse.Id
            ), cancellationToken
        ).ConfigureAwait(false);
    }
}
