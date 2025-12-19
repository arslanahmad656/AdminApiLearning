using Aro.UI.Application.DTOs;
using Aro.UI.Application.Exceptions;
using Aro.UI.Application.Interfaces;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace Aro.UI.Infrastructure.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly HttpClient _httpClient;
    private readonly ITokenStorageService _tokenStorage;

    public AuthenticationService(HttpClient httpClient, ITokenStorageService tokenStorage)
    {
        _httpClient = httpClient;
        _tokenStorage = tokenStorage;
    }

    public async Task<AuthenticationResponse?> LoginAsync(string email, string password, bool rememberMe = false)
    {
        try
        {

            var request = new AuthenticationRequest(email, password);

            var response = await _httpClient.PostAsJsonAsync("api/auth/authenticate", request);

            if (response.IsSuccessStatusCode)
            {
                var authResponse = await response.Content.ReadFromJsonAsync<AuthenticationResponse>();

                if (authResponse != null)
                {
                    await _tokenStorage.SetTokensAsync(authResponse.AccessToken, authResponse.RefreshToken, rememberMe);
                    return authResponse;
                }
            }
            else if (response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                // Account is locked - parse the lockout response
                var errorContent = await response.Content.ReadAsStringAsync();
                try
                {
                    var lockoutResponse = JsonSerializer.Deserialize<LockoutErrorResponse>(errorContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    throw new AccountLockedException(
                        lockoutResponse?.ErrorMessage ?? "Account is locked due to too many failed login attempts.",
                        lockoutResponse?.LockoutEnd);
                }
                catch (JsonException)
                {
                    // If we can't parse the response, throw a generic lockout exception
                    throw new AccountLockedException("Account is locked due to too many failed login attempts. Please try again later.");
                }
            }

            return null;
        }
        catch (HttpRequestException ex)
        {
            // Network-related errors (no internet, DNS failure, connection refused, etc.)
            throw new HttpRequestException("Unable to connect to the server. Please check your internet connection and try again.", ex);
        }
        catch (TaskCanceledException ex)
        {
            // Timeout errors
            throw new TaskCanceledException("The request timed out. Please check your internet connection and try again.", ex);
        }
        catch (Exception ex)
        {
            // Other unexpected errors - rethrow
            throw;
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

            var request = new RefreshTokenRequest(refreshToken);

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
        catch (HttpRequestException ex)
        {
            // Network-related errors (no internet, DNS failure, connection refused, etc.)
            throw new HttpRequestException("Unable to connect to the server. Please check your internet connection and try again.", ex);
        }
        catch (TaskCanceledException ex)
        {
            // Timeout errors
            throw new TaskCanceledException("The request timed out. Please check your internet connection and try again.", ex);
        }
        catch (Exception ex)
        {
            // Other unexpected errors - rethrow
            throw;
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

            // Try to get user ID from 'sub' claim
            if (!root.TryGetProperty("sub", out var subProperty))
            {
                return null;
            }

            // Try different claim names for email (email, Email, http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress)
            string email = string.Empty;
            if (root.TryGetProperty("email", out var emailProp))
            {
                email = emailProp.GetString() ?? string.Empty;
            }
            else if (root.TryGetProperty("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress", out var emailClaimProp))
            {
                email = emailClaimProp.GetString() ?? string.Empty;
            }

            // Try different claim names for name
            string displayName = string.Empty;
            if (root.TryGetProperty("name", out var nameProp))
            {
                displayName = nameProp.GetString() ?? string.Empty;
            }
            else if (root.TryGetProperty("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name", out var nameClaimProp))
            {
                displayName = nameClaimProp.GetString() ?? string.Empty;
            }

            var userId = Guid.Parse(subProperty.GetString() ?? Guid.Empty.ToString());
            var roles = new List<string>();
            var permissions = new List<string>();

            // Extract roles (try both 'role' and the full claim name)
            if (root.TryGetProperty("role", out var roleProperty))
            {
                if (roleProperty.ValueKind == JsonValueKind.Array)
                {
                    foreach (var role in roleProperty.EnumerateArray())
                    {
                        roles.Add(role.GetString() ?? string.Empty);
                    }
                }
                else if (roleProperty.ValueKind == JsonValueKind.String)
                {
                    roles.Add(roleProperty.GetString() ?? string.Empty);
                }
            }
            else if (root.TryGetProperty("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", out var roleClaimProperty))
            {
                if (roleClaimProperty.ValueKind == JsonValueKind.Array)
                {
                    foreach (var role in roleClaimProperty.EnumerateArray())
                    {
                        roles.Add(role.GetString() ?? string.Empty);
                    }
                }
                else if (roleClaimProperty.ValueKind == JsonValueKind.String)
                {
                    roles.Add(roleClaimProperty.GetString() ?? string.Empty);
                }
            }

            // Extract permissions
            if (root.TryGetProperty("permission", out var permissionProperty))
            {
                if (permissionProperty.ValueKind == JsonValueKind.Array)
                {
                    foreach (var permission in permissionProperty.EnumerateArray())
                    {
                        permissions.Add(permission.GetString() ?? string.Empty);
                    }
                }
                else if (permissionProperty.ValueKind == JsonValueKind.String)
                {
                    permissions.Add(permissionProperty.GetString() ?? string.Empty);
                }
            }

            var userInfo = new UserInfo(userId, email, displayName, roles, permissions);

            return userInfo;
        }
        catch (Exception ex)
        {
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
