namespace Aro.Admin.Application.Mediator.Authentication.DTOs;

public record AuthenticateUserRequest
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}

