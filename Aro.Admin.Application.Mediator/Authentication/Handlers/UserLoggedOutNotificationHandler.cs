using Aro.Admin.Application.Mediator.Authentication.Notifications;
using Aro.Admin.Application.Services.DataServices;
using Aro.Admin.Application.Services.DTOs.ServiceParameters.Audit;
using MediatR;

namespace Aro.Admin.Application.Mediator.Authentication.Handlers;

public class UserLoggedOutNotificationHandler(IAuditService auditService) : INotificationHandler<UserLoggedOutNotification>
{
    public async Task Handle(UserLoggedOutNotification notification, CancellationToken cancellationToken)
    {
        await auditService.LogUserSessionLoggedOutLog(new(notification.Data.UserId, notification.Data.RefreshTokenHash, notification.Data.TokenIdentifier), cancellationToken).ConfigureAwait(false);
    }
}
