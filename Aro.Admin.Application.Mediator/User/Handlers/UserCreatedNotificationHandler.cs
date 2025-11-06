using Aro.Admin.Application.Mediator.User.Notifications;
using Aro.Admin.Application.Services.DTOs.ServiceParameters.Audit;
using Aro.Common.Application.Services.Audit;
using MediatR;

namespace Aro.Admin.Application.Mediator.User.Handlers;

public class UserCreatedNotificationHandler(IAuditService auditService) : INotificationHandler<UserCreatedNotification>
{
    public async Task Handle(UserCreatedNotification notification, CancellationToken cancellationToken)
    {
        await auditService.LogUserCreated(new(notification.CreateUserResponse.Id, notification.CreateUserResponse.Email, notification.CreateUserResponse.AssignedRoles), cancellationToken).ConfigureAwait(false);
    }
}
