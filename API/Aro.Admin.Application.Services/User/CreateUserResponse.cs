namespace Aro.Admin.Application.Services.User;

public record CreateUserResponse(Guid Id, string? Email, IList<Guid>? AssignedRoles);
