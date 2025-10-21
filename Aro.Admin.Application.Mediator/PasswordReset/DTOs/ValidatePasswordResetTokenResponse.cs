namespace Aro.Admin.Application.Mediator.PasswordReset.DTOs;

public record ValidatePasswordResetTokenResponse(bool IsValid, Guid? UserId);
