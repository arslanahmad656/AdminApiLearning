using Aro.Admin.Application.Mediator.PasswordReset.Notifications;
using Aro.Admin.Application.Services;
using Aro.Admin.Application.Services.DataServices;
using Aro.Admin.Application.Services.DTOs.ServiceParameters.Audit;
using MediatR;

namespace Aro.Admin.Application.Mediator.PasswordReset.Handlers;

public class PasswordResetCompletedAuditHandler(
    IAuditService auditService,
    IRequestInterpretorService requestInterpretor) : INotificationHandler<PasswordResetCompletedNotification>
{
    public async Task Handle(PasswordResetCompletedNotification notification, CancellationToken cancellationToken)
    {
        var auditLog = new PasswordResetCompletedLog(
            notification.Data.UserId,
            notification.Data.ResetAt,
            requestInterpretor.RetrieveIpAddress() ?? string.Empty,
            requestInterpretor.GetUserAgent() ?? string.Empty
        );

        await auditService.LogPasswordResetCompleted(auditLog, cancellationToken).ConfigureAwait(false);
    }
}
