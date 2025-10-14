namespace Aro.Admin.Application.Mediator.Role.DTOs;

public record GetRolesByIdRequest
{
    public IEnumerable<Guid> RoleIds { get; init; } = Array.Empty<Guid>();
}
