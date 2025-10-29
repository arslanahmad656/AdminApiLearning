namespace Aro.Admin.Application.Mediator.PasswordReset.DTOs;

public record ResetPasswordResponse(bool Success, string? ErrorCode, string Message);
