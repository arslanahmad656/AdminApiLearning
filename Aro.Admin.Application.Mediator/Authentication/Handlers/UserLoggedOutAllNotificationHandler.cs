using Aro.Admin.Application.Mediator.Authentication.Notifications;
using Aro.Admin.Application.Services.DataServices;
using Aro.Admin.Application.Services.DTOs.ServiceParameters.Audit;
using AutoMapper;
using MediatR;

namespace Aro.Admin.Application.Mediator.Authentication.Handlers;

public class UserLoggedOutAllNotificationHandler(IAuditService auditService, IMapper mapper) : INotificationHandler<UserLoggedOutAllNotification>
{
    public async Task Handle(UserLoggedOutAllNotification notification, CancellationToken cancellationToken)
    {
        await auditService.LogUserSessionsLoggedOutLog(mapper.Map<UserSessionsLoggedOutLog>(notification.Data), cancellationToken).ConfigureAwait(false);
    }
}
