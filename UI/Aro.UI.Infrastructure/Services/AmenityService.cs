using Aro.UI.Application.DTOs.Room;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Aro.UI.Infrastructure.Services;

public class AmenityService(HttpClient httpClient) : IAmenityService
{
    private readonly HttpClient _httpClient = httpClient;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public async Task<Amenity?> GetAmenity(Guid amenityId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/amenity/get/{amenityId}");
            if (!response.IsSuccessStatusCode)
                return null;

            var result = await response.Content.ReadFromJsonAsync<GetAmenityResponse>(_jsonOptions);
            if (result?.Amenity == null)
                return null;

            return new Amenity
            {
                Id = result.Amenity.Id,
                Name = result.Amenity.Name
            };
        }
        catch
        {
            return null;
        }
    }

    public async Task<List<Amenity>> GetAmenities(List<Guid> amenityIds)
    {
        var amenities = new List<Amenity>();

        // Fetch amenities in parallel for better performance
        var tasks = amenityIds.Select(id => GetAmenity(id));
        var results = await Task.WhenAll(tasks);

        foreach (var amenity in results)
        {
            if (amenity != null)
            {
                amenities.Add(amenity);
            }
        }

        return amenities;
    }

    private record AmenityDto(Guid Id, string Name);
    private record GetAmenityResponse(AmenityDto Amenity);
}
