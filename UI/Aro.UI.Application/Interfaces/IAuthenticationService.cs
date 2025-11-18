using Aro.UI.Application.DTOs;

namespace Aro.UI.Application.Interfaces;

public interface IAuthenticationService
{
    Task<AuthenticationResponse?> LoginAsync(string email, string password, bool rememberMe = false);
    Task<AuthenticationResponse?> RefreshTokenAsync();
    Task LogoutAsync();
    Task<UserInfo?> GetCurrentUserAsync();
}
