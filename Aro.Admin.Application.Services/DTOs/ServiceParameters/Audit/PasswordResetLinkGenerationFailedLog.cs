namespace Aro.Admin.Application.Services.DTOs.ServiceParameters.Audit;

public record PasswordResetLinkGenerationFailedLog(string Email, string ErrorMessage, string ErrorCode, DateTime FailedAt);
