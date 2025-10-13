namespace Aro.Admin.Application.Services.DTOs.ServiceParameters.Audit;

public record TokenRefreshedLog
{
    public Guid UserId { get; init; }
    public string OldRefreshTokenHash { get; init; } = string.Empty;
    public string NewRefreshTokenHash { get; init; } = string.Empty;
    public DateTime NewAccessTokenExpiry { get; init; }
    public DateTime NewRefreshTokenExpiry { get; init; }
}

