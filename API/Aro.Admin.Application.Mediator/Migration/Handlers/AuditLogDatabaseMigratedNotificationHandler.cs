using Aro.Admin.Application.Mediator.Migration.Notifications;
using Aro.Common.Application.Services.Audit;
using MediatR;

namespace Aro.Admin.Application.Mediator.Migration.Handlers;

public class AuditLogDatabaseMigratedNotificationHandler(IAuditService auditService, AuditActions auditActions) : INotificationHandler<DatabaseMigratedNotification>
{
    public async Task Handle(DatabaseMigratedNotification notification, CancellationToken cancellationToken)
    {
        var log = new { };
        
        await auditService.Log(new AuditEntryDto(auditActions.MigrationsApplied, string.Empty, string.Empty, log), cancellationToken).ConfigureAwait(false);
    }
}
