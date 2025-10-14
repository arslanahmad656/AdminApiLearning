using Aro.Admin.Application.Mediator.SystemSettings.Notifications;
using Aro.Admin.Application.Services.DataServices;
using Aro.Admin.Application.Services.DTOs.ServiceParameters.Audit;
using MediatR;

namespace Aro.Admin.Application.Mediator.SystemSettings.Handlers;

public class SystemInitializedNotificationHandler(IAuditService auditService) : INotificationHandler<SystemInitializedNotification>
{
    public async Task Handle(SystemInitializedNotification notification, CancellationToken cancellationToken)
    {
        await auditService.LogSystemInitialized(new(notification.InitializeSystemResponse.BootstrapUserId, notification.InitializeSystemResponse.BootstrapUsername, notification.InitializeSystemResponse.BootstrapAdminRoleName), cancellationToken).ConfigureAwait(false);
    }
}
