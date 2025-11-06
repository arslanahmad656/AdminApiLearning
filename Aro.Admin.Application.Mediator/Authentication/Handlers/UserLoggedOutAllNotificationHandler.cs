using Aro.Admin.Application.Mediator.Authentication.Notifications;
using Aro.Common.Application.Services.Audit;
using MediatR;

namespace Aro.Admin.Application.Mediator.Authentication.Handlers;

public class UserLoggedOutAllNotificationHandler(IAuditService auditService, AuditActions auditActions) : INotificationHandler<UserLoggedOutAllNotification>
{
    public async Task Handle(UserLoggedOutAllNotification notification, CancellationToken cancellationToken)
    {
        var log = new
        {
            notification.Data.UserId,
        };

        await auditService.Log(new(auditActions.UserSessionLoggedOut, string.Empty, string.Empty, log), cancellationToken).ConfigureAwait(false);
    }
}
