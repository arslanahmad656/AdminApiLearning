using Aro.Admin.Application.Mediator.PasswordReset.Notifications;
using Aro.Admin.Application.Services;
using MediatR;

namespace Aro.Admin.Application.Mediator.PasswordReset.Handlers;

public class PasswordResetEmailFailedNotificationHandler(ILogManager<PasswordResetEmailFailedNotificationHandler> logger) : INotificationHandler<PasswordResetEmailFailedNotification>
{
    public Task Handle(PasswordResetEmailFailedNotification notification, CancellationToken cancellationToken)
    {
        logger.LogError("Password reset email failed to send to user: {Email}, error: {ErrorMessage}, errorCode: {ErrorCode}, failedAt: {FailedAt}",
            notification.Data.Email,
            notification.Data.ErrorMessage,
            notification.Data.ErrorCode,
            notification.Data.FailedAt);

        // TODO: Log security events, audit trail, alerting, etc.
        // This is where you would:
        // - Log security events for audit purposes
        // - Send alerts to administrators
        // - Track email delivery failures for monitoring
        // - Update user communication preferences if needed

        return Task.CompletedTask;
    }
}
