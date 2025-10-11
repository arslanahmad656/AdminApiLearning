namespace Aro.Admin.Application.Mediator.Authentication.DTOs;

public record UserLoggedOutNotificationData(Guid UserId, string RefreshTokenHash);

