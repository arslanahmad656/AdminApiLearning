namespace Aro.Admin.Application.Mediator.Authentication.DTOs;

public record RefreshTokenRequest
{
    public string RefreshToken { get; init; } = string.Empty;
}


