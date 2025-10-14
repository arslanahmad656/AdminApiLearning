namespace Aro.Admin.Application.Mediator.Role.DTOs;

public record GetRolesByIdRequest(IEnumerable<Guid> RoleIds);
