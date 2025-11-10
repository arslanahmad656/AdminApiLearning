namespace Aro.Admin.Presentation.UI.Models;

public record PasswordResetRequest(
    string Token,
    string NewPassword
);
