namespace Aro.Admin.Presentation.Api.DTOs;

public record AssignRolesModel(IEnumerable<Guid> UserIds, IEnumerable<Guid> RoleIds);

