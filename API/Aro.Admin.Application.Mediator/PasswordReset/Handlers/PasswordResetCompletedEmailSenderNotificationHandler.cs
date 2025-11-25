using Aro.Admin.Application.Mediator.PasswordReset.Notifications;
using Aro.Admin.Application.Services.Email;
using Aro.Admin.Application.Services.User;
using Aro.Admin.Application.Shared.Options;
using Aro.Common.Application.Services.SystemContext;
using MediatR;
using Microsoft.Extensions.Options;

namespace Aro.Admin.Application.Mediator.PasswordReset.Handlers;

public class PasswordResetCompletedEmailSenderNotificationHandler(IUserService userService, IEmailTemplateService emailTemplateService, IEmailService emailService, IOptions<PasswordResetSettings> passwordResetSettings, ISystemContextFactory systemContextFactory) : INotificationHandler<PasswordResetCompletedNotification>
{
    private readonly PasswordResetSettings passwordResetSettings = passwordResetSettings.Value;

    public async Task Handle(PasswordResetCompletedNotification notification, CancellationToken cancellationToken)
    {
        using var systemContext = systemContextFactory.Create();

        var data = notification.Data;

        var user = await userService.GetUserById(data.UserId, false, false, cancellationToken).ConfigureAwait(false);
        var emailTemplate = await emailTemplateService.GetPasswordResetSuccesfulEmail(user.DisplayName, passwordResetSettings.FrontendLoginUrl, data.ResetAt, cancellationToken).ConfigureAwait(false);
        var emailParameters = new SendEmailParameters(user.Email, emailTemplate.Subject, emailTemplate.Body, emailTemplate.IsHtml);

        await emailService.SendEmail(emailParameters, cancellationToken).ConfigureAwait(false);
    }
}
