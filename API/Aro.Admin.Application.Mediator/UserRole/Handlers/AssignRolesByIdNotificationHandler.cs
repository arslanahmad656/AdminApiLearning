using Aro.Admin.Application.Mediator.UserRole.Notifications;
using Aro.Common.Application.Services.Audit;
using Aro.Common.Domain.Shared;
using MediatR;

namespace Aro.Admin.Application.Mediator.UserRole.Handlers;

public class AssignRolesByIdNotificationHandler(IAuditService auditService, AuditActions auditActions, EntityTypes entityTypes) : INotificationHandler<AssignRolesByIdNotification>
{
    public async Task Handle(AssignRolesByIdNotification notification, CancellationToken cancellationToken)
    {
        var log = new
        {
            notification.RolesAssignedResponse.UserIds,
            notification.RolesAssignedResponse.RoleIds
        };

        await auditService.Log(new(auditActions.RolesAssignedToUsers, entityTypes.Role, string.Empty, log), cancellationToken).ConfigureAwait(false);
    }
}
