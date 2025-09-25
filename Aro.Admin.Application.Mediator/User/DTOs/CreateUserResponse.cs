namespace Aro.Admin.Application.Mediator.User.DTOs;

public record CreateUserResponse(Guid Id, string? Email, IList<Guid>? AssignedRoles);
