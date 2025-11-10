using Aro.Admin.Application.Mediator.Authentication.Notifications;
using Aro.Common.Application.Services.Audit;
using MediatR;

namespace Aro.Admin.Application.Mediator.Authentication.Handlers;

public class UserAuthenticatedNotificationHandler(IAuditService auditService, AuditActions auditActions) : INotificationHandler<UserAuthenticatedNotification>
{
    public async Task Handle(UserAuthenticatedNotification notification, CancellationToken cancellationToken)
    {
        var log = new
        {
            notification.Data.UserId,
            notification.Data.Email,
            notification.Data.RefreshTokenId,
            notification.Data.AccessTokenExpiry,
            notification.Data.RefreshTokenExpiry,
            notification.Data.AccessTokenIdentifier
        };

        await auditService.Log(new(auditActions.AuthenticationSuccessful, string.Empty, string.Empty, log), cancellationToken).ConfigureAwait(false);
    }
}
