using Aro.Admin.Application.Mediator.PasswordReset.Commands;
using Aro.Admin.Application.Mediator.PasswordReset.DTOs;
using Aro.Admin.Application.Mediator.PasswordReset.Notifications;
using Aro.Admin.Application.Services;
using Aro.Admin.Application.Services.DTOs.ServiceParameters.Email;
using Aro.Admin.Application.Services.DTOs.ServiceParameters.PasswordLink;
using Aro.Admin.Domain.Shared.Exceptions;
using MediatR;

namespace Aro.Admin.Application.Mediator.PasswordReset.Handlers;

public class SendPasswordResetEmailLinkCommandHandler(
    IPasswordResetLinkService passwordResetLinkService,
    IEmailService emailService,
    ILogManager<SendPasswordResetEmailLinkCommandHandler> logger,
    IMediator mediator) : IRequestHandler<SendPasswordResetEmailLinkCommand, bool>
{
    public async Task<bool> Handle(SendPasswordResetEmailLinkCommand request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInfo("Starting password reset email link generation for user {Email}", request.Data.Email);

            var linkParameters = new GenerateLinkParameters(request.Data.Email);
            var resetLink = await passwordResetLinkService.GenerateLink(linkParameters, cancellationToken).ConfigureAwait(false);

            logger.LogInfo("Generated password reset link for user {Email}. Link={ResetLink}", request.Data.Email, resetLink);

            var emailParameters = new SendEmailParameters(
                To: request.Data.Email,
                Subject: "Password Reset Request",
                Body: $@"
                    <html>
                    <body>
                        <h2>Password Reset Request</h2>
                        <p>You have requested to reset your password. Click the link below to reset your password:</p>
                        <p><a href=""{resetLink}"">Reset Password</a></p>
                        <p>If you did not request this password reset, please ignore this email.</p>
                        <p>This link will expire in 30 minutes for security reasons.</p>
                        <br>
                        <p>Best regards,<br>Administration Team</p>
                    </body>
                    </html>",
                IsHtml: true
            );

            await emailService.SendEmail(emailParameters, cancellationToken).ConfigureAwait(false);

            logger.LogInfo("Successfully sent password reset email to user {Email}", request.Data.Email);

            var notificationData = new PasswordResetEmailSentNotificationData(
                request.Data.Email,
                resetLink,
                DateTime.UtcNow
            );
            await mediator.Publish(new PasswordResetEmailSentNotification(notificationData), cancellationToken).ConfigureAwait(false);

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send password reset email to user {Email}", request.Data.Email);

            var errorCode = ex is AroException aroException ? aroException.ErrorCode : string.Empty;
            var failureNotificationData = new PasswordResetEmailFailedNotificationData(
                request.Data.Email,
                ex.Message,
                errorCode,
                DateTime.UtcNow
            );
            await mediator.Publish(new PasswordResetEmailFailedNotification(failureNotificationData), cancellationToken).ConfigureAwait(false);

            return false;
        }
    }
}
