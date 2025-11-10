namespace Aro.Admin.Application.Mediator.PasswordReset.DTOs;

public record PasswordResetEmailFailedNotificationData(string Email, string ErrorMessage, string ErrorCode, DateTime FailedAt);
