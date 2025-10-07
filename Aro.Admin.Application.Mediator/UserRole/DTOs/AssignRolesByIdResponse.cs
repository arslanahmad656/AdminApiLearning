namespace Aro.Admin.Application.Mediator.UserRole.DTOs;

public record AssignRolesByIdResponse(List<Guid> UserIds, List<Guid> RoleIds);
