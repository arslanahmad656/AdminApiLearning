using Aro.Admin.Presentation.UI.Models;

namespace Aro.Admin.Presentation.UI.Services;

public interface IAuthenticationService
{
    Task<AuthenticationResponse?> LoginAsync(string email, string password);
    Task<AuthenticationResponse?> RefreshTokenAsync();
    Task LogoutAsync();
    Task<UserInfo?> GetCurrentUserAsync();
}
