namespace Aro.Admin.Application.Services.DTOs.ServiceParameters;

public record CreateUserDto(string Email, bool IsActive, string Password, string DisplayName, ICollection<string> AssignedRoles);
