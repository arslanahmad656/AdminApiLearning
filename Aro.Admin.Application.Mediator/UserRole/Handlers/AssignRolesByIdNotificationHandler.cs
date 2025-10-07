using Aro.Admin.Application.Mediator.UserRole.Notifications;
using Aro.Admin.Application.Services.DataServices;
using Aro.Admin.Application.Services.DTOs.ServiceParameters.Audit;
using AutoMapper;
using MediatR;

namespace Aro.Admin.Application.Mediator.UserRole.Handlers;

public class AssignRolesByIdNotificationHandler(IAuditService auditService, IMapper mapper) : INotificationHandler<AssignRolesByIdNotification>
{
    public async Task Handle(AssignRolesByIdNotification notification, CancellationToken cancellationToken)
    {
        await auditService.LogRolesAssigned(mapper.Map<RolesAssignedLog>(notification.RolesAssignedResponse), cancellationToken).ConfigureAwait(false);
    }
}
