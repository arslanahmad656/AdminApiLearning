namespace Aro.Admin.Application.Services.DTOs.ServiceResponses;

public record TokenInfo
{
    public string TokenIdentifier { get; init; } = string.Empty;
    public DateTime Expiry { get; init; }
}
