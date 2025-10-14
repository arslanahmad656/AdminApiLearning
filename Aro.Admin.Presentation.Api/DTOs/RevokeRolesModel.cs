namespace Aro.Admin.Presentation.Api.DTOs;

public record RevokeRolesModel(IEnumerable<Guid> UserIds, IEnumerable<Guid> RoleIds);

