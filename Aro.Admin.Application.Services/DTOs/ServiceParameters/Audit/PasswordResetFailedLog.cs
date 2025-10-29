namespace Aro.Admin.Application.Services.DTOs.ServiceParameters.Audit;

public record PasswordResetFailedLog(
    Guid? UserId,
    string FailureReason,
    DateTime FailedAt,
    string RequestIpAddress,
    string UserAgent
);
