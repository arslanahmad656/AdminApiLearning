namespace Aro.Admin.Application.Services.DTOs.ServiceParameters.Audit;

public record UserCreatedLog(Guid Id, string? Email, IList<Guid>? AssignedRoles);