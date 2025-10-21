using Aro.Admin.Application.Mediator.PasswordReset.Notifications;
using Aro.Admin.Application.Services;
using MediatR;

namespace Aro.Admin.Application.Mediator.PasswordReset.Handlers;

public class PasswordResetCompletedNotificationHandler(ILogManager<PasswordResetCompletedNotificationHandler> logger) : INotificationHandler<PasswordResetCompletedNotification>
{
    public Task Handle(PasswordResetCompletedNotification notification, CancellationToken cancellationToken)
    {
        logger.LogInfo("Password reset completed for user: {UserId}, email: {Email}, resetAt: {ResetAt}",
            notification.Data.UserId,
            notification.Data.Email,
            notification.Data.ResetAt);

        // TODO: Send confirmation email, log security events, etc.
        // This is where you would:
        // - Send confirmation email to user
        // - Log security events for audit
        // - Invalidate any existing sessions if needed

        return Task.CompletedTask;
    }
}
