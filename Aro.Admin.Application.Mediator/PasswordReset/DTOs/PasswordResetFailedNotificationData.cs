namespace Aro.Admin.Application.Mediator.PasswordReset.DTOs;

public record PasswordResetFailedNotificationData(
    Guid? UserId,
    string FailureReason,
    DateTime FailedAt
);
