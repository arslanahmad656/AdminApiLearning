using Aro.Admin.Application.Mediator.PasswordReset.Notifications;
using Aro.Admin.Application.Services;
using MediatR;

namespace Aro.Admin.Application.Mediator.PasswordReset.Handlers;

public class PasswordResetEmailSentNotificationHandler(ILogManager<PasswordResetEmailSentNotificationHandler> logger) : INotificationHandler<PasswordResetEmailSentNotification>
{
    public Task Handle(PasswordResetEmailSentNotification notification, CancellationToken cancellationToken)
    {
        logger.LogInfo("Password reset email sent successfully to user: {Email}, resetLink: {ResetLink}, sentAt: {SentAt}",
            notification.Data.Email,
            notification.Data.ResetLink,
            notification.Data.SentAt);

        // TODO: Log security events, audit trail, etc.
        // This is where you would:
        // - Log security events for audit purposes
        // - Track email delivery metrics
        // - Update user communication preferences if needed

        return Task.CompletedTask;
    }
}
