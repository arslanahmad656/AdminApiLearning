namespace Aro.Admin.Application.Services.DTOs.ServiceParameters.Audit;

public record PasswordResetTokenGeneratedLog(
    Guid UserId,
    string UserEmail,
    string Token,
    DateTime GeneratedAt,
    DateTime ExpiresAt,
    string RequestIpAddress,
    string UserAgent
);
