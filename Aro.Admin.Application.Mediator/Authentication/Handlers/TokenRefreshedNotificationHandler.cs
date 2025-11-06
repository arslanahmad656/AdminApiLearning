using Aro.Admin.Application.Mediator.Authentication.Notifications;
using Aro.Common.Application.Services.Audit;
using Aro.Common.Domain.Shared;
using MediatR;

namespace Aro.Admin.Application.Mediator.Authentication.Handlers;

public class TokenRefreshedNotificationHandler(IAuditService auditService, AuditActions auditActions, EntityTypes entityTypes) : INotificationHandler<TokenRefreshedNotification>
{
    public async Task Handle(TokenRefreshedNotification notification, CancellationToken cancellationToken)
    {
        var log = new
        {
            notification.Data.UserId,
            notification.Data.OldRefreshTokenHash,
            notification.Data.NewRefreshTokenHash,
            notification.Data.NewRefreshTokenExpiry
        };

        await auditService.Log(
            new(
                auditActions.AccessTokenRefreshed,
                entityTypes.AccessToken,
                notification.Data.UserId.ToString(),
                log
            ), cancellationToken
        ).ConfigureAwait(false);
    }
}
