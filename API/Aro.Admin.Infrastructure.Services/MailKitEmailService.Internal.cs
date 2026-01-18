using Aro.Admin.Application.Services.Email;
using MimeKit;

namespace Aro.Admin.Infrastructure.Services;

public partial class MailKitEmailService
{
    private MimeMessage BuildMimeMessage(SendEmailParameters parameters, IEnumerable<(string Key, string Value)> headers)
    {
        logger.LogDebug("Building MIME message for email. Subject={Subject}", parameters.Subject);

        var msg = new MimeMessage();

        logger.LogDebug("Setting From address. FromName={FromName}, FromEmail={FromEmail}",
            emailSettings.FromName,
            emailSettings.FromEmail);
        msg.From.Add(new MailboxAddress(emailSettings.FromName, emailSettings.FromEmail));

        logger.LogDebug("Adding To recipients. PrimaryRecipient={To}", parameters.To);
        AddMailbox(msg.To, [parameters.To]);

        if (parameters.Cc?.Any() == true)
        {
            logger.LogDebug("Adding Cc recipients. CcCount={CcCount}", parameters.Cc.Count());
            AddMailbox(msg.Cc, parameters.Cc);
        }

        if (parameters.Bcc?.Any() == true)
        {
            logger.LogDebug("Adding Bcc recipients. BccCount={BccCount}", parameters.Bcc.Count());
            AddMailbox(msg.Bcc, parameters.Bcc);
        }

        msg.Subject = parameters.Subject;
        logger.LogDebug("Email subject set. Subject={Subject}", parameters.Subject);

        var bodyType = parameters.IsHtml ? "HTML" : "Plain Text";
        var bodyLength = parameters.Body?.Length ?? 0;
        logger.LogDebug("Building email body. BodyType={BodyType}, BodyLength={BodyLength}", bodyType, bodyLength);

        var builder = new BodyBuilder
        {
            HtmlBody = parameters.IsHtml ? parameters.Body : null,
            TextBody = parameters.IsHtml ? null : parameters.Body
        };

        if (parameters.Attachments is not null)
        {
            var attachmentCount = parameters.Attachments.Count();
            logger.LogDebug("Processing email attachments. AttachmentCount={AttachmentCount}", attachmentCount);

            var processedCount = 0;
            foreach (var att in parameters.Attachments)
            {
                if (att is null)
                {
                    logger.LogWarn("Skipping null attachment in attachments list");
                    continue;
                }

                logger.LogDebug("Adding attachment. FileName={FileName}, ContentType={ContentType}, DataSize={DataSize}",
                    att.FileName,
                    att.ContentType,
                    att.Data?.Length ?? 0);

                builder.Attachments.Add(att.FileName, att.Data, MimeKit.ContentType.Parse(att.ContentType));
                processedCount++;
            }

            logger.LogDebug("Attachments processed successfully. ProcessedCount={ProcessedCount}", processedCount);
        }
        else
        {
            logger.LogDebug("No attachments to process");
        }

        var headersList = headers.ToList();
        logger.LogDebug("Adding custom headers. HeaderCount={HeaderCount}", headersList.Count);

        var addedHeadersCount = 0;
        foreach (var (key, value) in headersList)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                logger.LogWarn("Skipping custom header with empty key");
                continue;
            }

            logger.LogDebug("Adding custom header. HeaderKey={HeaderKey}, HeaderValue={HeaderValue}",
                key.Trim(),
                value?.Trim() ?? string.Empty);

            msg.Headers.Add(key.Trim(), value?.Trim() ?? string.Empty);
            addedHeadersCount++;
        }

        logger.LogDebug("Custom headers added. AddedCount={AddedCount}", addedHeadersCount);

        msg.Body = builder.ToMessageBody();
        logger.LogDebug("MIME message body finalized");

        logger.LogDebug("MIME message built successfully. ToCount={ToCount}, CcCount={CcCount}, BccCount={BccCount}, HasAttachments={HasAttachments}",
            msg.To.Count,
            msg.Cc.Count,
            msg.Bcc.Count,
            parameters.Attachments?.Any() ?? false);

        return msg;
    }

    private void AddMailbox(InternetAddressList list, IEnumerable<string>? emails)
    {
        if (emails is null)
        {
            logger.LogDebug("No email addresses to add (null collection)");
            return;
        }

        var emailList = emails.ToList();
        logger.LogDebug("Adding mailbox addresses. AddressCount={AddressCount}", emailList.Count);

        var addedCount = 0;
        foreach (var e in emailList)
        {
            if (string.IsNullOrWhiteSpace(e))
            {
                logger.LogWarn("Skipping empty or whitespace email address");
                continue;
            }

            try
            {
                var trimmedEmail = e.Trim();
                list.Add(MailboxAddress.Parse(trimmedEmail));
                logger.LogDebug("Mailbox address added successfully. Email={Email}", trimmedEmail);
                addedCount++;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to parse email address. Email={Email}, ErrorMessage={ErrorMessage}",
                    e,
                    ex.Message);
                throw;
            }
        }

        logger.LogDebug("Mailbox addresses added. AddedCount={AddedCount}, TotalCount={TotalCount}",
            addedCount,
            emailList.Count);
    }
}
