namespace Aro.Admin.Application.Services.Role;

public record GetUserRolesResponse(Guid RoleId, string RoleName, string Description, bool IsBuiltIn);

