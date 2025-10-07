using Aro.Admin.Application.Mediator.UserRole.Notifications;
using Aro.Admin.Application.Services.DataServices;
using Aro.Admin.Application.Services.DTOs.ServiceParameters.Audit;
using AutoMapper;
using MediatR;

namespace Aro.Admin.Application.Mediator.UserRole.Handlers;

public class RevokeRolesByIdNotificationHandler(IAuditService auditService, IMapper mapper) : INotificationHandler<RevokeRolesByIdNotification>
{
    public async Task Handle(RevokeRolesByIdNotification notification, CancellationToken cancellationToken)
    {
        await auditService.LogRolesRevoked(mapper.Map<RolesRevokedLog>(notification.Data), cancellationToken).ConfigureAwait(false);
    }
}
