namespace Aro.Admin.Presentation.UI.Models;
public record GetUserResponse(
    Guid Id,
    string Email,
    bool IsActive,
    string DisplayName
    //string PasswordHash,
    //ICollection<string> AssignedRoles
    );
