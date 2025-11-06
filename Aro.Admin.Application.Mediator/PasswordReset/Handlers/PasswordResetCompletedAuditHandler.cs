using Aro.Admin.Application.Mediator.PasswordReset.Notifications;
using Aro.Common.Application.Services.Audit;
using Aro.Common.Application.Services.RequestInterpretor;
using MediatR;

namespace Aro.Admin.Application.Mediator.PasswordReset.Handlers;

public class PasswordResetCompletedAuditHandler(
    IAuditService auditService,
    IRequestInterpretorService requestInterpretor,
    AuditActions auditActions) : INotificationHandler<PasswordResetCompletedNotification>
{
    public async Task Handle(PasswordResetCompletedNotification notification, CancellationToken cancellationToken)
    {
        var log = new
        {
            notification.Data.UserId,
            notification.Data.ResetAt,
            IPAddress = requestInterpretor.RetrieveIpAddress() ?? string.Empty,
            UserAgent = requestInterpretor.GetUserAgent() ?? string.Empty
        };

        await auditService.Log(new(auditActions.PasswordResetCompleted, string.Empty, string.Empty, log), cancellationToken).ConfigureAwait(false);
    }
}
