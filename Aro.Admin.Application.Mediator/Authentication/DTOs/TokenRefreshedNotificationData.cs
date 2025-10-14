namespace Aro.Admin.Application.Mediator.Authentication.DTOs;

public record TokenRefreshedNotificationData
{
    public Guid UserId { get; init; }
    public string OldRefreshTokenHash { get; init; } = string.Empty;
    public string NewRefreshTokenHash { get; init; } = string.Empty;
    public DateTime NewAccessTokenExpiry { get; init; }
    public DateTime NewRefreshTokenExpiry { get; init; }
}

