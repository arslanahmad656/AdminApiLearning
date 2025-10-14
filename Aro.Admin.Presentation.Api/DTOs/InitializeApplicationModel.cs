namespace Aro.Admin.Presentation.Api.DTOs;

public record InitializeApplicationModel
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public string BootstrapPassword { get; init; } = string.Empty;
}

