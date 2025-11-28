using System.Net.Http.Json;
using System.Security.Cryptography;
using Aro.UI.Application.DTOs;

namespace Aro.UI.Infrastructure.Services;

public class UserService(HttpClient httpClient) : IUserService
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task<CreateUserResponse?> CreateUser(CreateUserRequest request)
    {
        // Temporary approach
        var tempPassword = GenerateSecurePassword();
        request = request with { Password = tempPassword, AssignedRoles = ["ClientAdmin"] };
        var response = await _httpClient.PostAsJsonAsync("api/user/create", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<CreateUserResponse>();
    }

    public async Task<GetUserResponse?> GetUserById(Guid Id)
    {
        var response = await _httpClient.GetAsync($"api/user/get-by-id/{Id}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<GetUserResponse>();
    }

    public async Task<GetUserResponse?> GetUserByEmail(GetUserByEmailRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync($"api/user/get-by-email", request);

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<GetUserResponse>();
    }

    static string GenerateSecurePassword(int length = 32)
    {
        const string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*?_-";
        var bytes = RandomNumberGenerator.GetBytes(length);
        var chars = bytes.Select(b => validChars[b % validChars.Length]);
        return new string([.. chars]);
    }
}
