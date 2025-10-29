namespace Aro.Admin.Application.Mediator.User.DTOs;

public record CreateUserRequest(
    string Email, 
    string PhoneNumber,
    bool IsActive, 
    string Password, 
    string DisplayName, 
    ICollection<string> AssignedRoles
);
