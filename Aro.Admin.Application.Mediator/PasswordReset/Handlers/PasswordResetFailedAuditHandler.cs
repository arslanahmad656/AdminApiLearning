using Aro.Admin.Application.Mediator.PasswordReset.Notifications;
using Aro.Admin.Application.Services;
using Aro.Admin.Application.Services.DataServices;
using Aro.Admin.Application.Services.DTOs.ServiceParameters.Audit;
using MediatR;

namespace Aro.Admin.Application.Mediator.PasswordReset.Handlers;

public class PasswordResetFailedAuditHandler(
    IAuditService auditService,
    IRequestInterpretorService requestInterpretor) : INotificationHandler<PasswordResetFailedNotification>
{
    public async Task Handle(PasswordResetFailedNotification notification, CancellationToken cancellationToken)
    {
        var auditLog = new PasswordResetFailedLog(
            notification.Data.UserId,
            notification.Data.FailureReason,
            notification.Data.FailedAt,
            requestInterpretor.RetrieveIpAddress() ?? string.Empty,
            requestInterpretor.GetUserAgent() ?? string.Empty
        );

        await auditService.LogPasswordResetFailed(auditLog, cancellationToken).ConfigureAwait(false);
    }
}
