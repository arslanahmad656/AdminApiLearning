using Aro.Admin.Application.Mediator.Authentication.Notifications;
using Aro.Admin.Application.Services.DataServices;
using MediatR;

namespace Aro.Admin.Application.Mediator.Authentication.Handlers;

public class UserLoggedOutAllNotificationHandler(IAuditService auditService) : INotificationHandler<UserLoggedOutAllNotification>
{
    public async Task Handle(UserLoggedOutAllNotification notification, CancellationToken cancellationToken)
    {
        await auditService.LogUserSessionsLoggedOutLog(new(notification.Data.UserId), cancellationToken).ConfigureAwait(false);
    }
}
