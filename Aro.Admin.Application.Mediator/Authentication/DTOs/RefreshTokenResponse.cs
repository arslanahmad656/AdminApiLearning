namespace Aro.Admin.Application.Mediator.Authentication.DTOs;

public record RefreshTokenResponse
{
    public Guid RefreshTokenId { get; init; }
    public string AccessToken { get; init; } = string.Empty;
    public string RefreshToken { get; init; } = string.Empty;
    public DateTime AccessTokenExpiry { get; init; }
    public DateTime RefreshTokenExpiry { get; init; }
}

