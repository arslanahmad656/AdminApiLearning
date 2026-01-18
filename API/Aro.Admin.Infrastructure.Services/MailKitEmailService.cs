using Aro.Admin.Application.Services.Email;
using Aro.Admin.Application.Shared.Options;
using Aro.Admin.Domain.Shared.Exceptions;
using Aro.Common.Application.Services.LogManager;
using Aro.Common.Domain.Shared;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;

namespace Aro.Admin.Infrastructure.Services;

public partial class MailKitEmailService(IOptionsSnapshot<EmailSettings> emailSettings, ILogManager<MailKitEmailService> logger, ErrorCodes errorCodes) : IEmailService
{
    private readonly EmailSettings emailSettings = emailSettings.Value;

    public async Task SendEmail(SendEmailParameters parameters, CancellationToken cancellationToken = default)
    {
        logger.LogInfo("Starting email send operation. To={To}, Subject={Subject}", parameters?.To ?? "null", parameters?.Subject ?? "null");

        ArgumentNullException.ThrowIfNull(parameters);
        if (string.IsNullOrWhiteSpace(parameters.To))
        {
            logger.LogWarn("Email send failed: Recipient 'To' is null or empty");
            throw new ArgumentException("Recipient 'To' must be provided.", nameof(parameters));
        }
        if (string.IsNullOrWhiteSpace(parameters.Subject))
        {
            logger.LogWarn("Email send failed: Subject is null or empty. To={To}", parameters.To);
            throw new ArgumentException("Email 'Subject' must be provided.", nameof(parameters));
        }

        var hasAttachments = parameters.Attachments is not null && parameters.Attachments.Any();
        var ccCount = parameters.Cc?.Count() ?? 0;
        var bccCount = parameters.Bcc?.Count() ?? 0;

        logger.LogDebug("Email parameters validated. To={To}, Subject={Subject}, IsHtml={IsHtml}, HasAttachments={HasAttachments}, CcCount={CcCount}, BccCount={BccCount}",
            parameters.To,
            parameters.Subject,
            parameters.IsHtml,
            hasAttachments,
            ccCount,
            bccCount);

        logger.LogDebug("Building MIME message. CustomHeadersCount={CustomHeadersCount}", emailSettings.CustomHeaders.Length);
        var message = BuildMimeMessage(parameters, emailSettings.CustomHeaders.Select(h => (h.Key, h.Value)));
        logger.LogDebug("MIME message built successfully");

        try
        {
            logger.LogDebug("Creating SMTP client for email delivery");
            using var smtp = new SmtpClient();

            var socketOptions = emailSettings.UseSsl
                ? SecureSocketOptions.SslOnConnect
                : emailSettings.UseStartTls
                    ? SecureSocketOptions.StartTls
                    : SecureSocketOptions.None;

            logger.LogInfo("Connecting to SMTP server. Host={Host}, Port={Port}, SocketOptions={SocketOptions}",
                emailSettings.Host,
                emailSettings.Port,
                socketOptions);

            await smtp.ConnectAsync(emailSettings.Host, emailSettings.Port, socketOptions, cancellationToken)
                      .ConfigureAwait(false);

            logger.LogDebug("Successfully connected to SMTP server. IsConnected={IsConnected}, IsSecure={IsSecure}",
                smtp.IsConnected,
                smtp.IsSecure);

            if (!string.IsNullOrWhiteSpace(emailSettings.UserName))
            {
                logger.LogInfo("Authenticating with SMTP server. UserName={UserName}", emailSettings.UserName);
                await smtp.AuthenticateAsync(emailSettings.UserName, emailSettings.Password, cancellationToken)
                          .ConfigureAwait(false);
                logger.LogDebug("Successfully authenticated with SMTP server. IsAuthenticated={IsAuthenticated}", smtp.IsAuthenticated);
            }
            else
            {
                logger.LogDebug("Skipping SMTP authentication (no username configured)");
            }

            logger.LogInfo("Sending email message via SMTP. To={To}, Subject={Subject}", parameters.To, parameters.Subject);
            await smtp.SendAsync(message, cancellationToken).ConfigureAwait(false);
            logger.LogDebug("Email message sent successfully via SMTP");

            logger.LogDebug("Disconnecting from SMTP server");
            await smtp.DisconnectAsync(true, cancellationToken).ConfigureAwait(false);
            logger.LogDebug("Disconnected from SMTP server");

            logger.LogInfo("Email sent successfully. To={To}, Subject={Subject}, From={FromEmail}, FromName={FromName}",
                parameters.To,
                parameters.Subject,
                emailSettings.FromEmail,
                emailSettings.FromName);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send email. To={To}, Subject={Subject}, Host={Host}, Port={Port}, ErrorMessage={ErrorMessage}",
                parameters.To,
                parameters.Subject,
                emailSettings.Host,
                emailSettings.Port,
                ex.Message);
            throw new AroEmailException(errorCodes.EMAIL_SENDING_ERROR, $"An error occurred while sending an email.", ex);
        }
    }
}
