namespace Aro.Admin.Application.Mediator.UserRole.DTOs;

public record RevokeRolesByIdResponse(List<Guid> UserIds, List<Guid> RoleIds);
