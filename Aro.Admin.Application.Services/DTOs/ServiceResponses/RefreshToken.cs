namespace Aro.Admin.Application.Services.DTOs.ServiceResponses;

public record RefreshToken
{
    public string Token { get; init; } = string.Empty;
    public DateTime ExpiresAt { get; init; }
}

