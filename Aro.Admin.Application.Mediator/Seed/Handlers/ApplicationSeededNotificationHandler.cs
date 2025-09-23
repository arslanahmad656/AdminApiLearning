using Aro.Admin.Application.Mediator.Seed.Notifications;
using Aro.Admin.Application.Services.DataServices;
using MediatR;

namespace Aro.Admin.Application.Mediator.Seed.Handlers;

public class ApplicationSeededNotificationHandler(IAuditService auditService) : INotificationHandler<ApplicationSeededNotification>
{
    public async Task Handle(ApplicationSeededNotification notification, CancellationToken cancellationToken)
    {
        await auditService.LogApplicationSeeded(cancellationToken);
    }
}
