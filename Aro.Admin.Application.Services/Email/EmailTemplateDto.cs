namespace Aro.Admin.Application.Services.Email;

public record EmailTemplateDto(string Identifier, string Subject, string Body, bool IsHtml);
