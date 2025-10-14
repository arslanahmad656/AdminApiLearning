namespace Aro.Admin.Application.Mediator.UserRole.DTOs;

public record GetUserRolesRequest
{
    public Guid UserId { get; init; }
}
