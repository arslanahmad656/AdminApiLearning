namespace Aro.Admin.Application.Services.DTOs.ServiceParameters.Audit;

public record AuthenticationFailedLog
{
    public string Email { get; init; } = string.Empty;
    public string ErrorMessage { get; init; } = string.Empty;
}

