namespace Aro.Admin.Application.Mediator.Authentication.DTOs;

public record LogoutUserRequest
{
    public Guid UserId { get; init; }
    public string RefreshToken { get; init; } = string.Empty;
}


