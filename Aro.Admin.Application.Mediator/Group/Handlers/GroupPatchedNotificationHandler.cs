using Aro.Admin.Application.Mediator.Group.Notifications;
using Aro.Admin.Application.Services.DTOs.ServiceParameters.Audit;
using Aro.Common.Application.Services.Audit;
using MediatR;

namespace Aro.Admin.Application.Mediator.Group.Handlers;

public class GroupPatchedNotificationHandler(IAuditService auditService) : INotificationHandler<GroupPatchedNotification>
{
    public async Task Handle(GroupPatchedNotification notification, CancellationToken cancellationToken)
    {
        await auditService.LogGroupPatched(
            new(
                notification.PatchGroupResponse.Id
            ), cancellationToken
        ).ConfigureAwait(false);
    }
}
