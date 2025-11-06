using Aro.Admin.Application.Mediator.Seed.Notifications;
using Aro.Common.Application.Services.Audit;
using MediatR;

namespace Aro.Admin.Application.Mediator.Seed.Handlers;

public class AuditLogApplicationSeededNotificationHandler(IAuditService auditService) : INotificationHandler<ApplicationSeededNotification>
{
    public async Task Handle(ApplicationSeededNotification notification, CancellationToken cancellationToken)
    {
        await auditService.LogApplicationSeeded(cancellationToken);
    }
}
