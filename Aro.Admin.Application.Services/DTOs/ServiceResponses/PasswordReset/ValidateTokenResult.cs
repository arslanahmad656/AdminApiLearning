namespace Aro.Admin.Application.Services.DTOs.ServiceResponses.PasswordReset;

public record ValidateTokenResult(
    bool IsValid,
    Guid? UserId,
    string? IpAddress,
    string? UserAgent,
    DateTime? Timestamp
);
