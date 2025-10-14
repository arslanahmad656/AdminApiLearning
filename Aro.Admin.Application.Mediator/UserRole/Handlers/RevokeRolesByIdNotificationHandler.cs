using Aro.Admin.Application.Mediator.UserRole.Notifications;
using Aro.Admin.Application.Services.DataServices;
using Aro.Admin.Application.Services.DTOs.ServiceParameters.Audit;
using MediatR;

namespace Aro.Admin.Application.Mediator.UserRole.Handlers;

public class RevokeRolesByIdNotificationHandler(IAuditService auditService) : INotificationHandler<RevokeRolesByIdNotification>
{
    public async Task Handle(RevokeRolesByIdNotification notification, CancellationToken cancellationToken)
    {
        await auditService.LogRolesRevoked(new(notification.Data.UserIds, notification.Data.RoleIds), cancellationToken).ConfigureAwait(false);
    }
}
