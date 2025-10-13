namespace Aro.Admin.Application.Mediator.Authentication.DTOs;

public record SuccessfulAuthenticationData
{
    public string Email { get; init; } = string.Empty;
    public string Token { get; init; } = string.Empty;
    public DateTime Expiry { get; init; }
    public string TokenIdentifier { get; init; } = string.Empty;
}

