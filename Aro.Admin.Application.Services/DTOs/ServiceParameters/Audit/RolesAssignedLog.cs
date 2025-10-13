namespace Aro.Admin.Application.Services.DTOs.ServiceParameters.Audit;

public record RolesAssignedLog
{
    public List<Guid> UserIds { get; init; } = new();
    public List<Guid> RoleIds { get; init; } = new();
}

