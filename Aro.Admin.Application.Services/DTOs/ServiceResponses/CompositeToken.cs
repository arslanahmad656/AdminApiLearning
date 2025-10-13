namespace Aro.Admin.Application.Services.DTOs.ServiceResponses;

public record CompositeToken
{
    public string OldRefreshTokenHash { get; init; } = string.Empty;
    public Guid UserId { get; init; }
    public Guid RefreshTokenId { get; init; }
    public string AccessToken { get; init; } = string.Empty;
    public string RefreshToken { get; init; } = string.Empty;
    public DateTime AccessTokenExpiry { get; init; }
    public DateTime RefreshTokenExpiry { get; init; }
    public string AccessTokenIdentifier { get; init; } = string.Empty;
}


