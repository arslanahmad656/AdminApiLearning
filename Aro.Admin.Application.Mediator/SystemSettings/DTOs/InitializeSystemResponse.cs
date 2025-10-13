namespace Aro.Admin.Application.Mediator.SystemSettings.DTOs;

public record InitializeSystemResponse
{
    public string BootstrapUserId { get; init; } = string.Empty;
    public string BootstrapUsername { get; init; } = string.Empty;
    public string BootstrapAdminRoleName { get; init; } = string.Empty;
}

