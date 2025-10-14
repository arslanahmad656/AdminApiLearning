namespace Aro.Admin.Application.Mediator.UserRole.DTOs;

public record RevokeRolesByIdRequest(IEnumerable<Guid> UserIds, IEnumerable<Guid> RoleIds);

