using Aro.Admin.Application.Mediator.PasswordReset.Notifications;
using Aro.Admin.Application.Services.Audit;
using Aro.Admin.Application.Services.DTOs.ServiceParameters.Audit;
using Aro.Admin.Application.Services.LogManager;
using Aro.Common.Application.Services.Audit;
using MediatR;

namespace Aro.Admin.Application.Mediator.PasswordReset.Handlers;

public class PasswordResetEmailFailedNotificationHandler(ILogManager<PasswordResetEmailFailedNotificationHandler> logger, IAuditService auditService) : INotificationHandler<PasswordResetEmailFailedNotification>
{
    public Task Handle(PasswordResetEmailFailedNotification notification, CancellationToken cancellationToken)
    {
        logger.LogError("Password reset email failed to send to user: {Email}, error: {ErrorMessage}, errorCode: {ErrorCode}, failedAt: {FailedAt}",
            notification.Data.Email,
            notification.Data.ErrorMessage,
            notification.Data.ErrorCode,
            notification.Data.FailedAt);
        var log = new PasswordResetLinkGenerationFailedLog(notification.Data.Email, notification.Data.ErrorMessage, notification.Data.ErrorCode, notification.Data.FailedAt);
        return auditService.LogPasswordResetLinkGenerationFailed(log, cancellationToken);
    }
}
