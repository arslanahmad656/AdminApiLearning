namespace Aro.Admin.Application.Services.DTOs.ServiceResponses;

public record GetRoleRespose(Guid Id, string Name, string Description, bool IsBuiltin);
