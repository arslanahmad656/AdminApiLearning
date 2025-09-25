namespace Aro.Admin.Application.Services.DTOs.ServiceResponses;

public record CreateUserResponse(Guid Id, string? Email, IList<Guid>? AssignedRoles);
