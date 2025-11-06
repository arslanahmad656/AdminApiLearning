using Aro.Admin.Application.Mediator.UserRole.Notifications;
using Aro.Admin.Application.Services.DTOs.ServiceParameters.Audit;
using Aro.Common.Application.Services.Audit;
using MediatR;

namespace Aro.Admin.Application.Mediator.UserRole.Handlers;

public class AssignRolesByIdNotificationHandler(IAuditService auditService) : INotificationHandler<AssignRolesByIdNotification>
{
    public async Task Handle(AssignRolesByIdNotification notification, CancellationToken cancellationToken)
    {
        await auditService.LogRolesAssigned(new(notification.RolesAssignedResponse.UserIds, notification.RolesAssignedResponse.RoleIds), cancellationToken).ConfigureAwait(false);
    }
}
