namespace Aro.Admin.Application.Services.DTOs.ServiceResponses;

public record AccessTokenResponse
{
    public string Token { get; init; } = string.Empty;
    public DateTime Expiry { get; init; }
    public string TokenIdentifier { get; init; } = string.Empty;
}
