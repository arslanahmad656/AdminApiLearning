namespace Aro.Admin.Application.Mediator.Shared.DTOs;

public record GetRoleResponse(Guid Id, string Name, string Description, bool IsBuiltin);


