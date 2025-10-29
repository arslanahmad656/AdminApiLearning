using Aro.Admin.Application.Mediator.PasswordReset.Commands;
using Aro.Admin.Application.Mediator.PasswordReset.DTOs;
using Aro.Admin.Application.Mediator.PasswordReset.Notifications;
using Aro.Admin.Application.Services;
using Aro.Admin.Application.Services.DataServices;
using Aro.Admin.Application.Services.DTOs.ServiceParameters.Email;
using Aro.Admin.Application.Services.DTOs.ServiceParameters.PasswordLink;
using Aro.Admin.Application.Services.SystemContext;
using Aro.Admin.Application.Shared.Options;
using Aro.Admin.Domain.Shared.Exceptions;
using MediatR;
using Microsoft.Extensions.Options;

namespace Aro.Admin.Application.Mediator.PasswordReset.Handlers;

public class SendPasswordResetEmailLinkCommandHandler(
    IPasswordResetLinkService passwordResetLinkService,
    IEmailService emailService,
    ILogManager<SendPasswordResetEmailLinkCommandHandler> logger,
    IMediator mediator,
    IEmailTemplateService emailTemplateService,
    IUserService userService,
    ISystemContextFactory systemContextFactory,
    IOptionsSnapshot<PasswordResetSettings> passwordResetSettings) : IRequestHandler<SendPasswordResetEmailLinkCommand, bool>
{
    private readonly PasswordResetSettings passwordResetSettings = passwordResetSettings.Value;
    public async Task<bool> Handle(SendPasswordResetEmailLinkCommand request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInfo("Starting password reset email link generation for user {Email}", request.Data.Email);

            using var systemContext = systemContextFactory.Create();

            return await HandleInternal(request, cancellationToken).ConfigureAwait(false);
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

    public async Task<bool> HandleInternal(SendPasswordResetEmailLinkCommand request, CancellationToken cancellationToken)
    {
        var linkParameters = new GenerateLinkParameters(request.Data.Email);
        var user = await userService.GetUserByEmail(request.Data.Email, false, false, cancellationToken).ConfigureAwait(false);
        var resetLink = await passwordResetLinkService.GenerateLink(linkParameters, cancellationToken).ConfigureAwait(false);

        logger.LogInfo("Generated password reset link for user {Email}. Link={ResetLink}", request.Data.Email, resetLink);

        var emailTemplate = await emailTemplateService.GetPasswordResetLinkEmail(user.DisplayName, resetLink.ToString(), passwordResetSettings.TokenExpiryMinutes, cancellationToken).ConfigureAwait(false);

        var emailParameters = new SendEmailParameters(
            To: request.Data.Email,
            Subject: emailTemplate.Subject,
            Body: emailTemplate.Body,
            IsHtml: emailTemplate.IsHtml
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
}
