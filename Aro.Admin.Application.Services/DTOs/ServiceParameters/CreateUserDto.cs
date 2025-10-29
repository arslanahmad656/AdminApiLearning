namespace Aro.Admin.Application.Services.DTOs.ServiceParameters;

public record CreateUserDto(
    string Email, 
    string PhoneNumber,
    bool IsActive, 
    bool IsSystemUser, 
    string Password, 
    string DisplayName, 
    ICollection<string> AssignedRoles
);

