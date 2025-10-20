using Aro.Admin.Presentation.UI.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace Aro.Admin.Presentation.UI.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly HttpClient _httpClient;
    private readonly ITokenStorageService _tokenStorage;

    public AuthenticationService(HttpClient httpClient, ITokenStorageService tokenStorage)
    {
        _httpClient = httpClient;
        _tokenStorage = tokenStorage;
    }

    public async Task<AuthenticationResponse?> LoginAsync(string email, string password)
    {
        try
        {
            var request = new AuthenticationRequest
            {
                Email = email,
                Password = password
            };

            var response = await _httpClient.PostAsJsonAsync("api/auth/authenticate", request);

            if (response.IsSuccessStatusCode)
            {
                var authResponse = await response.Content.ReadFromJsonAsync<AuthenticationResponse>();

                if (authResponse != null)
                {
                    await _tokenStorage.SetTokensAsync(authResponse.AccessToken, authResponse.RefreshToken);
                    return authResponse;
                }
            }

            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Login error: {ex.Message}");
            return null;
        }
    }

    public async Task<AuthenticationResponse?> RefreshTokenAsync()
    {
        try
        {
            var refreshToken = await _tokenStorage.GetRefreshTokenAsync();

            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return null;
            }

            var request = new RefreshTokenRequest
            {
                RefreshToken = refreshToken
            };

            var response = await _httpClient.PostAsJsonAsync("api/auth/refresh", request);

            if (response.IsSuccessStatusCode)
            {
                var authResponse = await response.Content.ReadFromJsonAsync<AuthenticationResponse>();

                if (authResponse != null)
                {
                    await _tokenStorage.SetTokensAsync(authResponse.AccessToken, authResponse.RefreshToken);
                    return authResponse;
                }
            }

            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Token refresh error: {ex.Message}");
            return null;
        }
    }

    public async Task LogoutAsync()
    {
        try
        {
            var refreshToken = await _tokenStorage.GetRefreshTokenAsync();

            if (!string.IsNullOrWhiteSpace(refreshToken))
            {
                var userInfo = await GetCurrentUserAsync();

                if (userInfo != null)
                {
                    var request = new
                    {
                        UserId = userInfo.UserId,
                        RefreshToken = refreshToken
                    };

                    // Call logout endpoint (don't wait for response)
                    _ = _httpClient.PostAsJsonAsync("api/auth/logout", request);
                }
            }

            // Clear tokens regardless of API call result
            await _tokenStorage.ClearTokensAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Logout error: {ex.Message}");
            // Always clear tokens on logout
            await _tokenStorage.ClearTokensAsync();
        }
    }

    public async Task<UserInfo?> GetCurrentUserAsync()
    {
        try
        {
            var accessToken = await _tokenStorage.GetAccessTokenAsync();

            if (string.IsNullOrWhiteSpace(accessToken))
            {
                return null;
            }

            // Decode JWT token to get user info
            var userInfo = DecodeJwtToken(accessToken);
            return userInfo;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Get user error: {ex.Message}");
            return null;
        }
    }

    private UserInfo? DecodeJwtToken(string token)
    {
        try
        {
            var parts = token.Split('.');
            if (parts.Length != 3)
            {
                return null;
            }

            var payload = parts[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var json = System.Text.Encoding.UTF8.GetString(jsonBytes);

            using var document = JsonDocument.Parse(json);
            var root = document.RootElement;

            var userInfo = new UserInfo
            {
                UserId = Guid.Parse(root.GetProperty("sub").GetString() ?? Guid.Empty.ToString()),
                Email = root.GetProperty("email").GetString() ?? string.Empty,
                DisplayName = root.TryGetProperty("name", out var name) ? name.GetString() ?? string.Empty : string.Empty
            };

            // Extract roles
            if (root.TryGetProperty("role", out var roleProperty))
            {
                if (roleProperty.ValueKind == JsonValueKind.Array)
                {
                    foreach (var role in roleProperty.EnumerateArray())
                    {
                        userInfo.Roles.Add(role.GetString() ?? string.Empty);
                    }
                }
                else if (roleProperty.ValueKind == JsonValueKind.String)
                {
                    userInfo.Roles.Add(roleProperty.GetString() ?? string.Empty);
                }
            }

            // Extract permissions
            if (root.TryGetProperty("permission", out var permissionProperty))
            {
                if (permissionProperty.ValueKind == JsonValueKind.Array)
                {
                    foreach (var permission in permissionProperty.EnumerateArray())
                    {
                        userInfo.Permissions.Add(permission.GetString() ?? string.Empty);
                    }
                }
                else if (permissionProperty.ValueKind == JsonValueKind.String)
                {
                    userInfo.Permissions.Add(permissionProperty.GetString() ?? string.Empty);
                }
            }

            return userInfo;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Token decode error: {ex.Message}");
            return null;
        }
    }

    private byte[] ParseBase64WithoutPadding(string base64)
    {
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }
        return Convert.FromBase64String(base64);
    }
}
