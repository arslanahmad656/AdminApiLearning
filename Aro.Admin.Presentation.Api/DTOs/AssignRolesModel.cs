namespace Aro.Admin.Presentation.Api.DTOs;

public record AssignRolesModel
{
    public IEnumerable<Guid> UserIds { get; init; } = Array.Empty<Guid>();
    public IEnumerable<Guid> RoleIds { get; init; } = Array.Empty<Guid>();
}

