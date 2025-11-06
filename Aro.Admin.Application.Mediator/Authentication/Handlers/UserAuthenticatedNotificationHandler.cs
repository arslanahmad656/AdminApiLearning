using Aro.Admin.Application.Mediator.Authentication.Notifications;
using Aro.Admin.Application.Services.DTOs.ServiceParameters.Audit;
using Aro.Common.Application.Services.Audit;
using MediatR;

namespace Aro.Admin.Application.Mediator.Authentication.Handlers;

public class UserAuthenticatedNotificationHandler(IAuditService auditService) : INotificationHandler<UserAuthenticatedNotification>
{
    public async Task Handle(UserAuthenticatedNotification notification, CancellationToken cancellationToken)
    {
        await auditService.LogAuthenticationSuccessful(new(notification.Data.UserId, notification.Data.Email, notification.Data.RefreshTokenId, notification.Data.AccessTokenExpiry, notification.Data.RefreshTokenExpiry, notification.Data.AccessTokenIdentifier), cancellationToken).ConfigureAwait(false);
    }
}
