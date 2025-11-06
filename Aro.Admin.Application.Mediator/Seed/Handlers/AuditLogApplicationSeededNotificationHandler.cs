using Aro.Admin.Application.Mediator.Seed.Notifications;
using Aro.Common.Application.Services.Audit;
using MediatR;

namespace Aro.Admin.Application.Mediator.Seed.Handlers;

public class AuditLogApplicationSeededNotificationHandler(IAuditService auditService, AuditActions auditActions) : INotificationHandler<ApplicationSeededNotification>
{
    public async Task Handle(ApplicationSeededNotification notification, CancellationToken cancellationToken)
    {
        var log = new { };
        await auditService.Log(new(auditActions.ApplicationSeeded, string.Empty, string.Empty, log), cancellationToken);
    }
}
