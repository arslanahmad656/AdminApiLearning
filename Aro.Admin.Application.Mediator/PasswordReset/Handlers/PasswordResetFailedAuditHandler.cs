using Aro.Admin.Application.Mediator.PasswordReset.Notifications;
using Aro.Common.Application.Services.Audit;
using Aro.Common.Application.Services.RequestInterpretor;
using MediatR;

namespace Aro.Admin.Application.Mediator.PasswordReset.Handlers;

public class PasswordResetFailedAuditHandler(
    IAuditService auditService,
    IRequestInterpretorService requestInterpretor,
    AuditActions auditActions) : INotificationHandler<PasswordResetFailedNotification>
{
    public async Task Handle(PasswordResetFailedNotification notification, CancellationToken cancellationToken)
    {
        var log = new
        {
            notification.Data.UserId,
            notification.Data.FailureReason,
            notification.Data.FailedAt,
            IPAddress = requestInterpretor.RetrieveIpAddress() ?? string.Empty,
            UserAgent = requestInterpretor.GetUserAgent() ?? string.Empty
        };

        await auditService.Log(new(auditActions.PasswordResetFailed, string.Empty, string.Empty, log), cancellationToken).ConfigureAwait(false);
    }
}
