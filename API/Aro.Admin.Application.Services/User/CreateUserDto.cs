namespace Aro.Admin.Application.Services.User;

public record CreateUserDto(
    string Email,
    bool IsActive, 
    bool IsSystemUser, 
    string Password, 
    string DisplayName, 
    ICollection<string> AssignedRoles
);

