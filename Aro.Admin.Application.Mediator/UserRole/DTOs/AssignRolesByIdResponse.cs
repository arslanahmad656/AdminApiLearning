namespace Aro.Admin.Application.Mediator.UserRole.DTOs;

public record AssignRolesByIdResponse
{
    public List<Guid> UserIds { get; init; } = new();
    public List<Guid> RoleIds { get; init; } = new();
}
