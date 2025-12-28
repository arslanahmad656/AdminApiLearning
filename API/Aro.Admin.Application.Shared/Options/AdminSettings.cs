namespace Aro.Admin.Application.Shared.Options;

public record AdminSettings
{
    public string AdminRoleName { get; init; } = string.Empty;

    public string BootstrapPassword { get; init; } = string.Empty;
}

