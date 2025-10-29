using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace Aro.Admin.Presentation.UI.Services;

public class ApiAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly IAuthenticationService _authenticationService;
    private readonly ITokenStorageService _tokenStorage;

    public ApiAuthenticationStateProvider(
        IAuthenticationService authenticationService,
        ITokenStorageService tokenStorage)
    {
        _authenticationService = authenticationService;
        _tokenStorage = tokenStorage;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            Console.WriteLine("ApiAuthenticationStateProvider: GetAuthenticationStateAsync called");
            var user = await _authenticationService.GetCurrentUserAsync();

            if (user == null)
            {
                Console.WriteLine("ApiAuthenticationStateProvider: No user found, returning anonymous");
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            Console.WriteLine($"ApiAuthenticationStateProvider: User found - {user.Email}");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.DisplayName),
                new Claim(ClaimTypes.Email, user.Email)
            };

            // Add roles
            foreach (var role in user.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Add permissions as claims
            foreach (var permission in user.Permissions)
            {
                claims.Add(new Claim("permission", permission));
            }

            var identity = new ClaimsIdentity(claims, "jwt");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            Console.WriteLine($"ApiAuthenticationStateProvider: Authentication state created with {claims.Count} claims");
            return new AuthenticationState(claimsPrincipal);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ApiAuthenticationStateProvider: Error - {ex.Message}");
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }
    }

    public async Task MarkUserAsAuthenticated()
    {
        var authState = await GetAuthenticationStateAsync();
        NotifyAuthenticationStateChanged(Task.FromResult(authState));
    }

    public void MarkUserAsLoggedOut()
    {
        var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
        var authState = Task.FromResult(new AuthenticationState(anonymousUser));
        NotifyAuthenticationStateChanged(authState);
    }
}
