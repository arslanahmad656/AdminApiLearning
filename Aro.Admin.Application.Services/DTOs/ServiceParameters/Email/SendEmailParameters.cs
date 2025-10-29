namespace Aro.Admin.Application.Services.DTOs.ServiceParameters.Email;

public record SendEmailParameters(string To, string Subject, string Body, bool IsHtml, IEnumerable<string>? Cc = null, IEnumerable<string>? Bcc = null, IEnumerable<EmailAttachement>? Attachments = null);
