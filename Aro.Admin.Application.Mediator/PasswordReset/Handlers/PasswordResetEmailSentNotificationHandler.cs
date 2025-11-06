using Aro.Admin.Application.Mediator.PasswordReset.Notifications;
using Aro.Common.Application.Services.Audit;
using Aro.Common.Application.Services.LogManager;
using Aro.Common.Domain.Shared;
using MediatR;

namespace Aro.Admin.Application.Mediator.PasswordReset.Handlers;

public class PasswordResetEmailSentNotificationHandler(ILogManager<PasswordResetEmailSentNotificationHandler> logger, IAuditService auditService, AuditActions auditActions, EntityTypes entityTypes) : INotificationHandler<PasswordResetEmailSentNotification>
{
    public Task Handle(PasswordResetEmailSentNotification notification, CancellationToken cancellationToken)
    {
        logger.LogInfo("Password reset email sent successfully to user: {Email}, resetLink: {ResetLink}, sentAt: {SentAt}",
            notification.Data.Email,
            notification.Data.ResetLink,
            notification.Data.SentAt);

        var log = new
        {
            notification.Data.Email,
            notification.Data.SentAt
        };

        return auditService.Log(new(auditActions.PasswordResetLinkGenerated, entityTypes.PasswordResetLink, string.Empty, log), cancellationToken);
    }
}
