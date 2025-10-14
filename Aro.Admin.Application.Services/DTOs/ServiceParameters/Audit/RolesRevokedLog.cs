namespace Aro.Admin.Application.Services.DTOs.ServiceParameters.Audit;

public record RolesRevokedLog(List<Guid> UserIds, List<Guid> RoleIds);


