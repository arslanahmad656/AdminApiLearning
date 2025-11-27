namespace Aro.UI.Application.DTOs;

public record UserInfo(
    Guid UserId,
    string Email,
    string DisplayName,
    List<string> Roles,
    List<string> Permissions
);
