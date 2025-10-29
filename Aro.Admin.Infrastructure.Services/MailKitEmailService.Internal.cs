using Aro.Admin.Application.Services.DTOs.ServiceParameters.Email;
using MimeKit;

namespace Aro.Admin.Infrastructure.Services;

public partial class MailKitEmailService
{
    private MimeMessage BuildMimeMessage(SendEmailParameters parameters)
    {
        var msg = new MimeMessage();

        msg.From.Add(new MailboxAddress(emailSettings.FromName, emailSettings.FromEmail));

        AddMailbox(msg.To, [parameters.To]);
        AddMailbox(msg.Cc, parameters.Cc);
        AddMailbox(msg.Bcc, parameters.Bcc);

        msg.Subject = parameters.Subject;

        var builder = new BodyBuilder
        {
            HtmlBody = parameters.IsHtml ? parameters.Body : null,
            TextBody = parameters.IsHtml ? null : parameters.Body
        };

        if (parameters.Attachments is not null)
        {
            foreach (var att in parameters.Attachments)
            {
                if (att is null) continue;
                builder.Attachments.Add(att.FileName, att.Data, MimeKit.ContentType.Parse(att.ContentType));
            }
        }

        msg.Body = builder.ToMessageBody();
        return msg;
    }

    private static void AddMailbox(InternetAddressList list, IEnumerable<string>? emails)
    {
        if (emails is null) return;

        foreach (var e in emails)
        {
            if (string.IsNullOrWhiteSpace(e)) continue;
            list.Add(MailboxAddress.Parse(e.Trim()));
        }
    }
}
