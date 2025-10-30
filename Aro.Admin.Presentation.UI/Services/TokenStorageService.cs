using Microsoft.JSInterop;

namespace Aro.Admin.Presentation.UI.Services;

public class TokenStorageService : ITokenStorageService
{
    private const string AccessTokenKey = "aro_access_token";
    private const string RefreshTokenKey = "aro_refresh_token";

    private readonly IJSRuntime _jsRuntime;

    public TokenStorageService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task<string?> GetAccessTokenAsync()
    {
        try
        {
            // Try localStorage first (rememberMe = true)
            var token = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", AccessTokenKey);

            if (!string.IsNullOrWhiteSpace(token))
            {
                return token;
            }

            // Then try sessionStorage (rememberMe = false)
            return await _jsRuntime.InvokeAsync<string?>("sessionStorage.getItem", AccessTokenKey);
        }
        catch
        {
            return null;
        }
    }

    public async Task<string?> GetRefreshTokenAsync()
    {
        try
        {
            // Try localStorage first (rememberMe = true)
            var token = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", RefreshTokenKey);

            if (!string.IsNullOrWhiteSpace(token))
            {
                return token;
            }

            // Then try sessionStorage (rememberMe = false)
            return await _jsRuntime.InvokeAsync<string?>("sessionStorage.getItem", RefreshTokenKey);
        }
        catch
        {
            return null;
        }
    }

    public async Task SetTokensAsync(string accessToken, string refreshToken, bool rememberMe = false)
    {
        try
        {

            // Clear tokens from both storages first to avoid duplicates
            await ClearTokensAsync();

            if (rememberMe)
            {
                // Store in localStorage (persists even after browser is closed)
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", AccessTokenKey, accessToken);
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", RefreshTokenKey, refreshToken);
            }
            else
            {
                // Store in sessionStorage (cleared when browser tab is closed)
                await _jsRuntime.InvokeVoidAsync("sessionStorage.setItem", AccessTokenKey, accessToken);
                await _jsRuntime.InvokeVoidAsync("sessionStorage.setItem", RefreshTokenKey, refreshToken);
            }

        }
        catch (Exception ex)
        {
        }
    }

    public async Task ClearTokensAsync()
    {
        // Clear from both localStorage and sessionStorage
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", AccessTokenKey);
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", RefreshTokenKey);
        await _jsRuntime.InvokeVoidAsync("sessionStorage.removeItem", AccessTokenKey);
        await _jsRuntime.InvokeVoidAsync("sessionStorage.removeItem", RefreshTokenKey);
    }

    public async Task<bool> HasValidTokensAsync()
    {
        var accessToken = await GetAccessTokenAsync();
        return !string.IsNullOrWhiteSpace(accessToken);
    }
}
