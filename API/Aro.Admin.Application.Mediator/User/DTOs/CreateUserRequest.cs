namespace Aro.Admin.Application.Mediator.User.DTOs;

public record CreateUserRequest(
    string Email,
    bool IsActive,
    string Password,
    string DisplayName,
    ICollection<string> AssignedRoles,
    string CountryCode,
    string PhoneNumber
);
