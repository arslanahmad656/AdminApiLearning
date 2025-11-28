using Aro.Common.Application.Mediator.FileResource.Notifications;
using Aro.Common.Application.Services.Audit;
using Aro.Common.Domain.Shared;
using MediatR;

namespace Aro.Common.Application.Mediator.FileResource.Handlers;

public class FileCreatedNotificationHandler(
    IAuditService auditService,
    AuditActions auditActions,
    EntityTypes entityTypes
) : INotificationHandler<FileCreatedNotification>
{
    public async Task Handle(FileCreatedNotification notification, CancellationToken cancellationToken)
    {
        var log = new
        {
            notification.Id,
            notification.Name,
            notification.Uri
        };

        await auditService.Log(
            new(
                auditActions.FileCreated,
                entityTypes.FileResource,
                notification.Id.ToString(),
                log
            ),
            cancellationToken
        ).ConfigureAwait(false);
    }
}

