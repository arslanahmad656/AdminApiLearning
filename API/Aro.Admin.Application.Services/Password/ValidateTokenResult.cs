namespace Aro.Admin.Application.Services.Password;

public record ValidateTokenResult(
    bool IsValid,
    Guid? UserId,
    string? IpAddress,
    string? UserAgent,
    DateTime? Timestamp
);
