using Aro.Admin.Application.Mediator.PasswordReset.Notifications;
using Aro.Admin.Application.Services;
using Aro.Admin.Application.Services.DataServices;
using Aro.Admin.Application.Services.DTOs.ServiceParameters.Audit;
using MediatR;

namespace Aro.Admin.Application.Mediator.PasswordReset.Handlers;

public class PasswordResetEmailSentNotificationHandler(ILogManager<PasswordResetEmailSentNotificationHandler> logger, IAuditService auditService) : INotificationHandler<PasswordResetEmailSentNotification>
{
    public Task Handle(PasswordResetEmailSentNotification notification, CancellationToken cancellationToken)
    {
        logger.LogInfo("Password reset email sent successfully to user: {Email}, resetLink: {ResetLink}, sentAt: {SentAt}",
            notification.Data.Email,
            notification.Data.ResetLink,
            notification.Data.SentAt);

        var log = new PasswordResetLinkGeneratedLog(notification.Data.Email, notification.Data.SentAt);
        return auditService.LogPasswordResetLinkGenerated(log, cancellationToken);
    }
}
