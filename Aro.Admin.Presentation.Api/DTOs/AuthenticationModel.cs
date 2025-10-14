namespace Aro.Admin.Presentation.Api.DTOs;

public record AuthenticationModel
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}

