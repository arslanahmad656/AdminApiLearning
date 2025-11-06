using Aro.Admin.Application.Mediator.User.Notifications;
using Aro.Common.Application.Services.Audit;
using Aro.Common.Domain.Shared;
using MediatR;

namespace Aro.Admin.Application.Mediator.User.Handlers;

public class ChangePasswordResetSuccessNotificationHandler(IAuditService auditService, AuditActions auditActions, EntityTypes entityTypes) : INotificationHandler<ChangePasswordSuccessNotification>
{
    public async Task Handle(ChangePasswordSuccessNotification notification, CancellationToken cancellationToken)
    {
        var log = new
        {
            notification.Data.Email
        };

        await auditService.Log(new(auditActions.PasswordChangeSuccess, entityTypes.User, notification.Data.Email, log), cancellationToken).ConfigureAwait(false);
    }
}
