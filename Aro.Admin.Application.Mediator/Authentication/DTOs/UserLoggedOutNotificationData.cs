namespace Aro.Admin.Application.Mediator.Authentication.DTOs;

public record UserLoggedOutNotificationData
{
    public Guid UserId { get; init; }
    public string RefreshTokenHash { get; init; } = string.Empty;
    public string TokenIdentifier { get; init; } = string.Empty;
}

