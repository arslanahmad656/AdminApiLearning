using Aro.Admin.Application.Mediator.Authentication.Notifications;
using Aro.Common.Application.Services.Audit;
using Aro.Common.Domain.Shared;
using MediatR;

namespace Aro.Admin.Application.Mediator.Authentication.Handlers;

public class UserLoggedOutNotificationHandler(IAuditService auditService, AuditActions auditActions, EntityTypes entityTypes) : INotificationHandler<UserLoggedOutNotification>
{
    public async Task Handle(UserLoggedOutNotification notification, CancellationToken cancellationToken)
    {
        var log = new
        {
            notification.Data.UserId,
            notification.Data.RefreshTokenHash,
            notification.Data.TokenIdentifier
        };

        await auditService.Log(new(auditActions.UserSessionLoggedOut, entityTypes.User, notification.Data.UserId.ToString(), log), cancellationToken).ConfigureAwait(false);
    }
}
