namespace Aro.Admin.Application.Services.DTOs.ServiceParameters.Audit;

public record RolesAssignedLog(List<Guid> UserIds, List<Guid> RoleIds);
