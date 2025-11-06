using Aro.Admin.Application.Mediator.Authentication.Notifications;
using Aro.Admin.Application.Services.DTOs.ServiceParameters.Audit;
using Aro.Common.Application.Services.Audit;
using MediatR;

namespace Aro.Admin.Application.Mediator.Authentication.Handlers;

public class UserAuthenticationFailedNotificationHandler(IAuditService auditService) : INotificationHandler<UserAuthenticationFailedNotification>
{
    public async Task Handle(UserAuthenticationFailedNotification notification, CancellationToken cancellationToken)
    {
        await auditService.LogAuthenticationFailed(new(notification.Data.Email, notification.Data.ErrorMessage), cancellationToken).ConfigureAwait(false);
    }
}
