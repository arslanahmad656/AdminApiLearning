using Aro.UI.Application.Interfaces;
using Aro.UI.Application.DTOs;
using System.Net.Http.Json;

namespace Aro.UI.Infrastructure.Services;

public class PropertyService : IPropertyService
{
    private readonly HttpClient _httpClient;

    public PropertyService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<CreatePropertyResponse?> CreatePropertyAsync(CreatePropertyRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/property/create", request);

            if (response.IsSuccessStatusCode)
            {
                var propertyResponse = await response.Content.ReadFromJsonAsync<CreatePropertyResponse>();
                return propertyResponse;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error creating property: {errorContent}");
            }

            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception creating property: {ex.Message}");
            return null;
        }
    }
}
