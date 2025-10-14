namespace Aro.Admin.Application.Mediator.UserRole.DTOs;

public record UserHasRoleRequest
{
    public Guid UserId { get; init; }
    public Guid RoleId { get; init; }
}

