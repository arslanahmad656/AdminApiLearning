namespace Aro.Admin.Application.Mediator.Authentication.DTOs;

public record FailedAuthenticationData
{
    public string Email { get; init; } = string.Empty;
    public string ErrorMessage { get; init; } = string.Empty;
}
