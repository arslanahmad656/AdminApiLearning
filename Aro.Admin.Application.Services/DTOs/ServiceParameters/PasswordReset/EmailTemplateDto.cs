namespace Aro.Admin.Application.Services.DTOs.ServiceParameters.PasswordReset;

public record EmailTemplateDto(string Identifier, string Subject, string Body, bool IsHtml);
