using Aro.Admin.Application.Mediator.Authentication.Notifications;
using Aro.Admin.Application.Services.DataServices;
using Aro.Admin.Application.Services.DTOs.ServiceParameters.Audit;
using MediatR;

namespace Aro.Admin.Application.Mediator.Authentication.Handlers;

public class TokenRefreshedNotificationHandler(IAuditService auditService) : INotificationHandler<TokenRefreshedNotification>
{
    public async Task Handle(TokenRefreshedNotification notification, CancellationToken cancellationToken)
    {
        await auditService.LogTokenRefreshedLog(new(notification.Data.UserId, notification.Data.OldRefreshTokenHash, notification.Data.NewRefreshTokenHash, notification.Data.NewRefreshTokenExpiry, notification.Data.NewRefreshTokenExpiry), cancellationToken).ConfigureAwait(false);
    }
}
