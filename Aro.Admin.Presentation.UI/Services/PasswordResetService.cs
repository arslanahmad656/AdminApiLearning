using Aro.Admin.Presentation.UI.Models;
using System.Net.Http.Json;

namespace Aro.Admin.Presentation.UI.Services;

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
            Console.WriteLine($"PasswordResetService: Sending reset link to {email}");

            var request = new SendPasswordResetLinkRequest
            {
                Email = email
            };

            var response = await _httpClient.PostAsJsonAsync("api/password-reset/send-reset-link", request);
            Console.WriteLine($"PasswordResetService: API response status: {response.StatusCode}");

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("PasswordResetService: Reset link sent successfully");
                return true;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"PasswordResetService: API error - {errorContent}");
                return false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"PasswordResetService: Send reset link error - {ex.Message}");
            return false;
        }
    }

    public async Task<PasswordResetResponse?> ResetPasswordAsync(string token, string newPassword)
    {
        try
        {
            Console.WriteLine("PasswordResetService: Resetting password with token");

            var request = new PasswordResetRequest
            {
                Token = token,
                NewPassword = newPassword
            };

            var response = await _httpClient.PostAsJsonAsync("api/password-reset/reset", request);
            Console.WriteLine($"PasswordResetService: API response status: {response.StatusCode}");

            if (response.IsSuccessStatusCode)
            {
                var resetResponse = await response.Content.ReadFromJsonAsync<PasswordResetResponse>();
                Console.WriteLine($"PasswordResetService: Password reset response - Success: {resetResponse?.Success}");
                return resetResponse;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"PasswordResetService: API error - {errorContent}");
                return new PasswordResetResponse
                {
                    Success = false,
                    Message = "Failed to reset password. Please try again."
                };
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"PasswordResetService: Reset password error - {ex.Message}");
            return new PasswordResetResponse
            {
                Success = false,
                Message = "An error occurred. Please try again."
            };
        }
    }
}
