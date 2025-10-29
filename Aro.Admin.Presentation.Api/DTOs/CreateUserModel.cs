namespace Aro.Admin.Presentation.Api.DTOs;

public record CreateUserModel(
    string Email,
    string PhoneNumber,
    bool IsActive, 
    string Password, 
    string DisplayName, 
    ICollection<string> AssignedRoles
);

