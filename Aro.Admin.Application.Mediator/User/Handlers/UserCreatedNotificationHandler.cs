using Aro.Admin.Application.Mediator.User.Notifications;
using Aro.Common.Application.Services.Audit;
using Aro.Common.Domain.Shared;
using MediatR;

namespace Aro.Admin.Application.Mediator.User.Handlers;

public class UserCreatedNotificationHandler(IAuditService auditService, AuditActions auditActions, EntityTypes entityTypes) : INotificationHandler<UserCreatedNotification>
{
    public async Task Handle(UserCreatedNotification notification, CancellationToken cancellationToken)
    {
        var log = new
        {
            notification.CreateUserResponse.Id,
            notification.CreateUserResponse.Email,
            notification.CreateUserResponse.AssignedRoles
        };

        await auditService.Log(new(auditActions.UserCreated, entityTypes.User, notification.CreateUserResponse.Id.ToString(), log), cancellationToken).ConfigureAwait(false);
    }
}
