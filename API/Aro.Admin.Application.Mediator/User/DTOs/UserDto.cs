namespace Aro.Admin.Application.Mediator.User.DTOs;

public record UserDto(
    Guid Id,
    string Email,
    bool IsActive,
    string DisplayName,
    string PasswordHash,
    IEnumerable<GetRoleResponse> Roles,
    ContactInfo ContactInfo);
