namespace Aro.Admin.Application.Mediator.Authentication.DTOs;

public record TokenRefreshedNotificationData(Guid UserId, string OldRefreshTokenHash, string NewRefreshTokenHash, DateTime NewAccessTokenExpiry, DateTime NewRefreshTokenExpiry);

