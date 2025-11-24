namespace Aro.Admin.Application.Mediator.User.DTOs;

public record GetRoleResponse(Guid Id, string Name, string Description, bool IsBuiltin);