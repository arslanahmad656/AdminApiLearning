namespace Aro.Admin.Application.Mediator.Authentication.DTOs;

public record AuthenticateUserResponse
{
    public Guid RefreshTokenId { get; init; }
    public string AccessToken { get; init; } = string.Empty;
    public string RefreshToken { get; init; } = string.Empty;
    public DateTime AccessTokenExpiry { get; init; }
    public DateTime RefreshTokenExpiry { get; init; }
    public string AccessTokenIdentifier { get; init; } = string.Empty;
}
