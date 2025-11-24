using Aro.Admin.Application.Services.Role;

namespace Aro.Admin.Application.Services.User;

public record UserDto(
    Guid Id,
    string Email,
    bool IsActive,
    string DisplayName,
    string PasswordHash,
    IEnumerable<GetRoleRespose> Roles,
    ContactInfoDto ContactInfo);



