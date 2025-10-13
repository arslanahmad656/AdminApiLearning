namespace Aro.Admin.Application.Services.DTOs.ServiceParameters.Audit;

public record UserSessionLoggedOutLog
{
    public Guid UserId { get; init; }
    public string RefreshTokenHash { get; init; } = string.Empty;
    public string TokenIdentifier { get; init; } = string.Empty;
}

