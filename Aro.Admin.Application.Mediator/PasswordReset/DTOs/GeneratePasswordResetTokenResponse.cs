namespace Aro.Admin.Application.Mediator.PasswordReset.DTOs;

public record GeneratePasswordResetTokenResponse(string Token, DateTime Expiry);
