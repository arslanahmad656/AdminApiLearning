namespace Aro.Admin.Application.Services.Role;

public record GetUserResponse(Guid Id, string Email, bool IsActive, string DisplayName, string PasswordHash, IEnumerable<GetRoleRespose> Roles);

