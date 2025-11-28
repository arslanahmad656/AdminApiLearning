namespace Aro.UI.Application.DTOs;
public record GetUserResponse(
    Guid Id,
    string Email,
    bool IsActive,
    string DisplayName
    //string PasswordHash,
    //ICollection<string> AssignedRoles
    );
