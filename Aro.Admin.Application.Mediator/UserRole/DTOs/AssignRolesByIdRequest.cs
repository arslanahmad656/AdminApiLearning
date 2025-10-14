namespace Aro.Admin.Application.Mediator.UserRole.DTOs;

public record AssignRolesByIdRequest(IEnumerable<Guid> UserIds, IEnumerable<Guid> RoleIds);


