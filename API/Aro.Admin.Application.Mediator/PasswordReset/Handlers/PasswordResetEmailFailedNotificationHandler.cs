using Aro.Admin.Application.Mediator.PasswordReset.Notifications;
using Aro.Common.Application.Services.Audit;
using Aro.Common.Application.Services.LogManager;
using Aro.Common.Domain.Shared;
using MediatR;

namespace Aro.Admin.Application.Mediator.PasswordReset.Handlers;

public class PasswordResetEmailFailedNotificationHandler(ILogManager<PasswordResetEmailFailedNotificationHandler> logger, IAuditService auditService, AuditActions auditActions, EntityTypes entityTypes) : INotificationHandler<PasswordResetEmailFailedNotification>
{
    public Task Handle(PasswordResetEmailFailedNotification notification, CancellationToken cancellationToken)
    {
        logger.LogError("Password reset email failed to send to user: {Email}, error: {ErrorMessage}, errorCode: {ErrorCode}, failedAt: {FailedAt}",
            notification.Data.Email,
            notification.Data.ErrorMessage,
            notification.Data.ErrorCode,
            notification.Data.FailedAt);

        var log = new
        {
            notification.Data.Email,
            notification.Data.ErrorMessage,
            notification.Data.ErrorCode,
            notification.Data.FailedAt,
        };

        return auditService.Log(new(auditActions.PasswordResetLinkGenerationFailed, entityTypes.PasswordResetLink, string.Empty, log), cancellationToken);
    }
}
