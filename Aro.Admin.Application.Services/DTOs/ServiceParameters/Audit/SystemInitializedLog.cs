namespace Aro.Admin.Application.Services.DTOs.ServiceParameters.Audit;

public record SystemInitializedLog
{
    public string BootstrapUserId { get; init; } = string.Empty;
    public string BootstrapUsername { get; init; } = string.Empty;
    public string BootstrapAdminRoleName { get; init; } = string.Empty;
}

