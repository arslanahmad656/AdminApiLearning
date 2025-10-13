namespace Aro.Admin.Presentation.Api.DTOs;

public record GetBootstrapUserModel
{
    public string BootstrapPassword { get; init; } = string.Empty;
}
