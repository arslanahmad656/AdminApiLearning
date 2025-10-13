using Aro.Admin.Application.Mediator.Migration.Notifications;
using Aro.Admin.Application.Services.DataServices;
using MediatR;

namespace Aro.Admin.Application.Mediator.Migration.Handlers;

public class AuditLogDatabaseMigratedNotificationHandler(IAuditService auditService) : INotificationHandler<DatabaseMigratedNotification>
{
    public async Task Handle(DatabaseMigratedNotification notification, CancellationToken cancellationToken)
    {
        await auditService.LogMigrationsCompleted(cancellationToken).ConfigureAwait(false);
    }
}
