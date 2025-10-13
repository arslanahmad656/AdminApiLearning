namespace Aro.Admin.Application.Services.DTOs.ServiceParameters.Audit;

public record RolesRevokedLog
{
    public List<Guid> UserIds { get; init; } = new();
    public List<Guid> RoleIds { get; init; } = new();
}


