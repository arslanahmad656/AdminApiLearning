//using Aro.Admin.Application.Mediator.PasswordReset.Notifications;
//using Aro.Admin.Application.Services;
//using MediatR;

//namespace Aro.Admin.Application.Mediator.PasswordReset.Handlers;

//public class PasswordResetTokenValidatedNotificationHandler(ILogManager<PasswordResetTokenValidatedNotificationHandler> logger) : INotificationHandler<PasswordResetTokenValidatedNotification>
//{
//    public Task Handle(PasswordResetTokenValidatedNotification notification, CancellationToken cancellationToken)
//    {
//        logger.LogInfo("Password reset token validation attempted for user: {UserId}, token: {Token}, isValid: {IsValid}",
//            notification.Data.UserId,
//            notification.Data.Token,
//            notification.Data.IsValid);

//        // TODO: Log security events, audit trail, etc.
//        // This is where you would log security events for audit purposes

//        return Task.CompletedTask;
//    }
//}
