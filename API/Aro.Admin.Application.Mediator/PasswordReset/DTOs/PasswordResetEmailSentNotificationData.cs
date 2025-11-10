namespace Aro.Admin.Application.Mediator.PasswordReset.DTOs;

public record PasswordResetEmailSentNotificationData(string Email, Uri ResetLink, DateTime SentAt);
