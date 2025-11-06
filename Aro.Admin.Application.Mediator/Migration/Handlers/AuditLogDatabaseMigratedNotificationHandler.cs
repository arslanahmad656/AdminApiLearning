using Aro.Admin.Application.Mediator.Migration.Notifications;
using Aro.Common.Application.Services.Audit;
using MediatR;

namespace Aro.Admin.Application.Mediator.Migration.Handlers;

public class AuditLogDatabaseMigratedNotificationHandler(IAuditService auditService) : INotificationHandler<DatabaseMigratedNotification>
{
    public async Task Handle(DatabaseMigratedNotification notification, CancellationToken cancellationToken)
    {
        await auditService.LogMigrationsCompleted(cancellationToken).ConfigureAwait(false);
    }
}
