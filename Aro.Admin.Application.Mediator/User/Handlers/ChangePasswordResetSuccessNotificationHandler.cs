using Aro.Admin.Application.Mediator.User.Notifications;
using Aro.Common.Application.Services.Audit;
using MediatR;

namespace Aro.Admin.Application.Mediator.User.Handlers;

public class ChangePasswordResetSuccessNotificationHandler(IAuditService auditService) : INotificationHandler<ChangePasswordSuccessNotification>
{
    public async Task Handle(ChangePasswordSuccessNotification notification, CancellationToken cancellationToken)
    {
        await auditService.LogPasswordChangedSuccess(new(notification.Data.Email), cancellationToken).ConfigureAwait(false);
    }
}
