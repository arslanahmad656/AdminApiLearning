namespace Aro.UI.Application.DTOs;

public record GetUserResponse(
    UserDto User
    );

public record UserDto(
    Guid Id,
    string Email,
    bool IsActive,
    string DisplayName
//string PasswordHash,
//ICollection<string> AssignedRoles
);
