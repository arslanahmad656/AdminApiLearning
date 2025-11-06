using Aro.Admin.Application.Services.Email;
using Aro.Admin.Application.Services.LogManager;
using Aro.Admin.Application.Shared.Options;
using Aro.Admin.Domain.Shared.Exceptions;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;

namespace Aro.Admin.Infrastructure.Services;

public partial class MailKitEmailService(IOptionsSnapshot<EmailSettings> emailSettings, ILogManager<MailKitEmailService> logger, ErrorCodes errorCodes) : IEmailService
{
    private readonly EmailSettings emailSettings = emailSettings.Value;

    public async Task SendEmail(SendEmailParameters parameters, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(parameters);
        if (string.IsNullOrWhiteSpace(parameters.To))
            throw new ArgumentException("Recipient 'To' must be provided.", nameof(parameters));
        if (string.IsNullOrWhiteSpace(parameters.Subject))
            throw new ArgumentException("Email 'Subject' must be provided.", nameof(parameters));

        var message = BuildMimeMessage(parameters);

        try
        {
            using var smtp = new SmtpClient();

            var socketOptions = emailSettings.UseSsl
                ? SecureSocketOptions.SslOnConnect
                : emailSettings.UseStartTls
                    ? SecureSocketOptions.StartTls
                    : SecureSocketOptions.None;

            await smtp.ConnectAsync(emailSettings.Host, emailSettings.Port, socketOptions, cancellationToken)
                      .ConfigureAwait(false);

            if (!string.IsNullOrWhiteSpace(emailSettings.UserName))
            {
                await smtp.AuthenticateAsync(emailSettings.UserName, emailSettings.Password, cancellationToken)
                          .ConfigureAwait(false);
            }

            await smtp.SendAsync(message, cancellationToken).ConfigureAwait(false);
            await smtp.DisconnectAsync(true, cancellationToken).ConfigureAwait(false);

            logger.LogInfo("Email sent. To={To}, Subject={Subject}", parameters.To, parameters.Subject);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send email. To={To}, Subject={Subject}", parameters.To, parameters.Subject);
            throw new AroEmailException(errorCodes.EMAIL_SENDING_ERROR, $"An error occurred while sending an email.", ex);
        }
    }
}
