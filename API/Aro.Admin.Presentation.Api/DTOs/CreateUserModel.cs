namespace Aro.Admin.Presentation.Api.DTOs;

public record CreateUserModel(
    string Email,
    bool IsActive,
    string Password,
    string DisplayName,
    ICollection<string> AssignedRoles,
    string? CountryCode,
    string? PhoneNumber
);

