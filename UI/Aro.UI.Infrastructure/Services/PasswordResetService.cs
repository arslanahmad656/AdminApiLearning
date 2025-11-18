using Aro.UI.Application.Interfaces;
using Aro.UI.Application.DTOs;
using System.Net.Http.Json;

namespace Aro.UI.Infrastructure.Services;

public class PasswordResetService : IPasswordResetService
{
    private readonly HttpClient _httpClient;

    public PasswordResetService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> SendPasswordResetLinkAsync(string email)
    {
        try
        {

            var request = new SendPasswordResetLinkRequest(email);

            var response = await _httpClient.PostAsJsonAsync("api/password-reset/send-reset-link", request);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return false;
            }
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public async Task<PasswordResetResponse?> ResetPasswordAsync(string token, string newPassword)
    {
        try
        {

            var request = new PasswordResetRequest(token, newPassword);

            var response = await _httpClient.PostAsJsonAsync("api/password-reset/reset", request);

            if (response.IsSuccessStatusCode)
            {
                var resetResponse = await response.Content.ReadFromJsonAsync<PasswordResetResponse>();
                return resetResponse;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return new PasswordResetResponse(false, null, "Failed to reset password. Please try again.");
            }
        }
        catch (Exception ex)
        {
            return new PasswordResetResponse(false, null, "An error occurred. Please try again.");
        }
    }
}
