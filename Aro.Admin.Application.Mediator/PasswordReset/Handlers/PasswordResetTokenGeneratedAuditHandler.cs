//using Aro.Admin.Application.Mediator.PasswordReset.Notifications;
//using Aro.Admin.Application.Services;
//using Aro.Admin.Application.Services.DataServices;
//using Aro.Admin.Application.Services.DTOs.ServiceParameters.Audit;
//using MediatR;

//namespace Aro.Admin.Application.Mediator.PasswordReset.Handlers;

//public class PasswordResetTokenGeneratedAuditHandler(
//    IAuditService auditService,
//    IRequestInterpretorService requestInterpretorService) : INotificationHandler<PasswordResetTokenGeneratedNotification>
//{
//    public async Task Handle(PasswordResetTokenGeneratedNotification notification, CancellationToken cancellationToken)
//    {
//        var ipAddress = requestInterpretorService.RetrieveIpAddress() ?? string.Empty;
//        var userAgent = requestInterpretorService.GetUserAgent() ?? string.Empty;

//        var auditLog = new PasswordResetTokenGeneratedLog(
//            notification.Data.UserId,
//            notification.Data.Email,
//            notification.Data.Token,
//            DateTime.UtcNow,
//            notification.Data.Expiry,
//            ipAddress,
//            userAgent
//        );

//        await auditService.LogPasswordResetTokenGenerated(auditLog, cancellationToken).ConfigureAwait(false);
//    }
//}
