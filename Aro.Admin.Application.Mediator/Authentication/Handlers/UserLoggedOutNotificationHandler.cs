using Aro.Admin.Application.Mediator.Authentication.Notifications;
using Aro.Admin.Application.Services.DataServices;
using Aro.Admin.Application.Services.DTOs.ServiceParameters.Audit;
using AutoMapper;
using MediatR;

namespace Aro.Admin.Application.Mediator.Authentication.Handlers;

public class UserLoggedOutNotificationHandler(IAuditService auditService, IMapper mapper) : INotificationHandler<UserLoggedOutNotification>
{
    public async Task Handle(UserLoggedOutNotification notification, CancellationToken cancellationToken)
    {
        await auditService.LogUserSessionLoggedOutLog(mapper.Map<UserSessionLoggedOutLog>(notification.Data), cancellationToken).ConfigureAwait(false);
    }
}
