using Aro.UI.Application.DTOs;

namespace Aro.UI.Application.Interfaces;

public interface IPasswordResetService
{
    Task<bool> SendPasswordResetLinkAsync(string email);
    Task<PasswordResetResponse?> ResetPasswordAsync(string token, string newPassword);
}
