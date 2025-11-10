using Aro.Admin.Application.Mediator.Authentication.Notifications;
using Aro.Common.Application.Services.Audit;
using MediatR;

namespace Aro.Admin.Application.Mediator.Authentication.Handlers;

public class UserAuthenticationFailedNotificationHandler(IAuditService auditService, AuditActions auditActions) : INotificationHandler<UserAuthenticationFailedNotification>
{
    public async Task Handle(UserAuthenticationFailedNotification notification, CancellationToken cancellationToken)
    {
        var log = new
        {
            notification.Data.Email,
            notification.Data.ErrorMessage
        };

        await auditService.Log(new(auditActions.AuthenticationFailed, string.Empty, string.Empty, log), cancellationToken).ConfigureAwait(false);
    }
}
