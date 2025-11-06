using Aro.Admin.Application.Mediator.UserRole.Notifications;
using Aro.Common.Application.Services.Audit;
using Aro.Common.Domain.Shared;
using MediatR;

namespace Aro.Admin.Application.Mediator.UserRole.Handlers;

public class RevokeRolesByIdNotificationHandler(IAuditService auditService, AuditActions auditActions, EntityTypes entityTypes) : INotificationHandler<RevokeRolesByIdNotification>
{
    public async Task Handle(RevokeRolesByIdNotification notification, CancellationToken cancellationToken)
    {
        var log = new
        {
            notification.Data.UserIds,
            notification.Data.RoleIds
        };

        await auditService.Log(new(auditActions.RolesRevokedFromUsers, entityTypes.Role, string.Empty, log), cancellationToken).ConfigureAwait(false);
    }
}
