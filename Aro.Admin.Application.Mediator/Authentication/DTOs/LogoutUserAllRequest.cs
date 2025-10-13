namespace Aro.Admin.Application.Mediator.Authentication.DTOs;

public record LogoutUserAllRequest
{
    public Guid UserId { get; init; }
}


