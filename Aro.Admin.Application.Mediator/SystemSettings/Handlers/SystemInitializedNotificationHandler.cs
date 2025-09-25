using Aro.Admin.Application.Mediator.SystemSettings.Notifications;
using Aro.Admin.Application.Services.DataServices;
using Aro.Admin.Application.Services.DTOs.ServiceParameters.Audit;
using AutoMapper;
using MediatR;

namespace Aro.Admin.Application.Mediator.SystemSettings.Handlers;

public class SystemInitializedNotificationHandler(IAuditService auditService, IMapper mapper) : INotificationHandler<SystemInitializedNotification>
{
    public async Task Handle(SystemInitializedNotification notification, CancellationToken cancellationToken)
    {
        await auditService.LogSystemInitialized(mapper.Map<SystemInitializedLog>(notification.InitializeSystemResponse), cancellationToken).ConfigureAwait(false);
    }
}
