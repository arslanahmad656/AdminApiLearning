namespace Aro.Admin.Application.Mediator.PasswordReset.DTOs;

public record ResetPasswordRequest(string Token, string NewPassword);
