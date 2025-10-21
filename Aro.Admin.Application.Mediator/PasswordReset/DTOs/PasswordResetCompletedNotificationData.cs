namespace Aro.Admin.Application.Mediator.PasswordReset.DTOs;

public record PasswordResetCompletedNotificationData(Guid UserId, string Email, DateTime ResetAt);
