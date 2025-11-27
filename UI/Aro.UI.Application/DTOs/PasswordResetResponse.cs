namespace Aro.UI.Application.DTOs;

public record PasswordResetResponse(
    bool Success,
    string? ErrorCode,
    string Message
);
