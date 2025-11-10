using Aro.Admin.Application.Mediator.SystemSettings.Notifications;
using Aro.Common.Application.Services.Audit;
using MediatR;

namespace Aro.Admin.Application.Mediator.SystemSettings.Handlers;

public class SystemInitializedNotificationHandler(IAuditService auditService, AuditActions auditActions) : INotificationHandler<SystemInitializedNotification>
{
    public async Task Handle(SystemInitializedNotification notification, CancellationToken cancellationToken)
    {
        var log = new
        {
            notification.InitializeSystemResponse.BootstrapUserId,
            notification.InitializeSystemResponse.BootstrapUsername,
            notification.InitializeSystemResponse.BootstrapAdminRoleName
        };

        await auditService.Log(new(auditActions.SystemInitialized, string.Empty, string.Empty, log), cancellationToken).ConfigureAwait(false);
    }
}
