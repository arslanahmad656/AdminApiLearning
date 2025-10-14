namespace Aro.Admin.Application.Mediator.UserRole.DTOs;

public record RevokeRolesByIdResponse
{
    public List<Guid> UserIds { get; init; } = new();
    public List<Guid> RoleIds { get; init; } = new();
}
