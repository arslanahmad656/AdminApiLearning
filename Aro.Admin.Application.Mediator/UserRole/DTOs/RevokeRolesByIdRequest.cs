namespace Aro.Admin.Application.Mediator.UserRole.DTOs;

public record RevokeRolesByIdRequest
{
    public IEnumerable<Guid> UserIds { get; init; } = Array.Empty<Guid>();
    public IEnumerable<Guid> RoleIds { get; init; } = Array.Empty<Guid>();
}

