namespace Aro.Admin.Application.Services.DTOs.ServiceParameters.Audit;

public record AuthenticationSuccessfulLog
{
    public Guid UserId { get; init; }
    public string Email { get; init; } = string.Empty;
    public Guid RefreshTokenId { get; init; }
    public DateTime AccessTokenExpiry { get; init; }
    public DateTime RefreshTokenExpiry { get; init; }
    public string AccessTokenIdentifier { get; init; } = string.Empty;
}


