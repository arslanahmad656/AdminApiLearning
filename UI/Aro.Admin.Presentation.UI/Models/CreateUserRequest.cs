namespace Aro.Admin.Presentation.UI.Models;

public record CreateUserRequest(
    string Email,
    string Password,
    bool IsActive,
    string DisplayName,
    ICollection<string> AssignedRoles,
    string CountryCode,
    string PhoneNumber
    );
