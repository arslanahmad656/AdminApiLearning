using Aro.Admin.Presentation.UI.Models;

namespace Aro.Admin.Presentation.UI.Services;

public interface IPasswordResetService
{
    Task<bool> SendPasswordResetLinkAsync(string email);
    Task<PasswordResetResponse?> ResetPasswordAsync(string token, string newPassword);
}
