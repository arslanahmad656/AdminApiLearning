namespace Aro.UI.Application.Interfaces;

public interface ITokenStorageService
{
    Task<string?> GetAccessTokenAsync();
    Task<string?> GetRefreshTokenAsync();
    Task SetTokensAsync(string accessToken, string refreshToken, bool rememberMe = false);
    Task ClearTokensAsync();
    Task<bool> HasValidTokensAsync();
}
