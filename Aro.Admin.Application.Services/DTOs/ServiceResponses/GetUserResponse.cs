namespace Aro.Admin.Application.Services.DTOs.ServiceResponses;

public record GetUserResponse(Guid Id, string Email, bool IsActive, string DisplayName, IEnumerable<GetRoleRespose> Roles);
