namespace Aro.Admin.Presentation.UI.Models;

public record UserInfo(
    Guid UserId,
    string Email,
    string DisplayName,
    List<string> Roles,
    List<string> Permissions
);
