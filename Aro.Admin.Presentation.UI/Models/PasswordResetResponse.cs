namespace Aro.Admin.Presentation.UI.Models;

public record PasswordResetResponse(
    bool Success,
    string? ErrorCode,
    string Message
);
