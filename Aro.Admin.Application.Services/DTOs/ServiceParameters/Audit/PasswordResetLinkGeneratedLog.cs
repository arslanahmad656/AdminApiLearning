namespace Aro.Admin.Application.Services.DTOs.ServiceParameters.Audit;

public record PasswordResetLinkGeneratedLog(string Email, DateTime GeneratedAt);
