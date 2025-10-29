namespace Aro.Admin.Application.Mediator.PasswordReset.DTOs;

public record PasswordResetCompletedNotificationData(Guid UserId, DateTime ResetAt);
