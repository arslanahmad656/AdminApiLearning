using Aro.Admin.Application.Mediator.PasswordReset.Notifications;
using Aro.Admin.Application.Services;
using MediatR;

namespace Aro.Admin.Application.Mediator.PasswordReset.Handlers;

public class PasswordResetTokenGeneratedNotificationHandler(ILogManager<PasswordResetTokenGeneratedNotificationHandler> logger) : INotificationHandler<PasswordResetTokenGeneratedNotification>
{
    public Task Handle(PasswordResetTokenGeneratedNotification notification, CancellationToken cancellationToken)
    {
        logger.LogInfo("Sending password reset email for user: {UserId}, email: {Email}, token: {Token}, expiry: {Expiry}",
            notification.Data.UserId,
            notification.Data.Email,
            notification.Data.Token,
            notification.Data.Expiry);

        // TODO: Send email with password reset link
        // This is where you would integrate with your email service
        // to send the password reset email to the user

        return Task.CompletedTask;
    }
}
