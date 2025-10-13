namespace Aro.Admin.Application.Services.DTOs.ServiceParameters.Audit;

public record AuthenticationSuccessfulLog
{
    public string Email { get; init; } = string.Empty;
    public DateTime Expiry { get; init; }
    public string TokenIdentifier { get; init; } = string.Empty;
}


