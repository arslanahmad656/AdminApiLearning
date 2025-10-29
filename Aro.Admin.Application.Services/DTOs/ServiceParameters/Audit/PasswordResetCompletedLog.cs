namespace Aro.Admin.Application.Services.DTOs.ServiceParameters.Audit;

public record PasswordResetCompletedLog(
    Guid UserId,
    DateTime CompletedAt,
    string RequestIpAddress,
    string UserAgent
);
