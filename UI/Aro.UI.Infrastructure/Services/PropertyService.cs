using Aro.UI.Application.Interfaces;
using Aro.UI.Application.DTOs;
using System.Net.Http.Json;
using System.Net.Http.Headers;

namespace Aro.UI.Infrastructure.Services;

public class PropertyService : IPropertyService
{
    private readonly HttpClient _httpClient;

    public PropertyService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<CreatePropertyResponse?> CreatePropertyAsync(PropertyWizardModel wizardData)
    {
        try
        {
            using var formData = new MultipartFormDataContent();

            // Add scalar fields
            formData.Add(new StringContent(wizardData.GroupId?.ToString() ?? ""), "GroupId");
            formData.Add(new StringContent(wizardData.PropertyName ?? ""), "PropertyName");
            formData.Add(new StringContent(wizardData.StarRating.ToString()), "StarRating");
            formData.Add(new StringContent(wizardData.Currency ?? ""), "Currency");
            formData.Add(new StringContent(wizardData.Description ?? ""), "Description");

            // Add property types as list
            foreach (var propertyType in wizardData.PropertyTypes.ToList())
            {
                formData.Add(new StringContent(propertyType), "PropertyTypes");
            }

            // Add address fields
            formData.Add(new StringContent(wizardData.SetAddressSameAsGroupAddress.ToString().ToLower()), "SetAddressSameAsGroupAddress");
            formData.Add(new StringContent(wizardData.AddressLine1 ?? ""), "AddressLine1");
            formData.Add(new StringContent(wizardData.AddressLine2 ?? ""), "AddressLine2");
            formData.Add(new StringContent(wizardData.City ?? ""), "City");
            formData.Add(new StringContent(wizardData.Country ?? ""), "Country");
            formData.Add(new StringContent(wizardData.PostalCode ?? ""), "PostalCode");
            formData.Add(new StringContent(wizardData.PhoneNumber ?? ""), "PhoneNumber");
            formData.Add(new StringContent(wizardData.Website ?? ""), "Website");

            // Add contact fields
            formData.Add(new StringContent(wizardData.SetContactSameAsPrimaryContact.ToString().ToLower()), "SetContactSameAsPrimaryContact");
            formData.Add(new StringContent(wizardData.ContactName ?? ""), "ContactName");
            formData.Add(new StringContent(wizardData.ContactEmail ?? ""), "ContactEmail");

            // Add key selling points as list
            foreach (var point in wizardData.KeySellingPoints ?? new List<string>())
            {
                formData.Add(new StringContent(point), "KeySellingPoints");
            }

            // Add marketing fields
            formData.Add(new StringContent(wizardData.MarketingTitle ?? ""), "MarketingTitle");
            formData.Add(new StringContent(wizardData.MarketingDescription ?? ""), "MarketingDescription");

            // Add file uploads
            if (wizardData.Favicon != null && !string.IsNullOrEmpty(wizardData.Favicon.Base64Data))
            {
                var fileContent = new ByteArrayContent(wizardData.Favicon.GetBytes());
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(wizardData.Favicon.ContentType);
                formData.Add(fileContent, "Files.Favicon", wizardData.Favicon.FileName);
            }

            if (wizardData.Banner1 != null && !string.IsNullOrEmpty(wizardData.Banner1.Base64Data))
            {
                var fileContent = new ByteArrayContent(wizardData.Banner1.GetBytes());
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(wizardData.Banner1.ContentType);
                formData.Add(fileContent, "Files.Banner1", wizardData.Banner1.FileName);
            }

            if (wizardData.Banner2 != null && !string.IsNullOrEmpty(wizardData.Banner2.Base64Data))
            {
                var fileContent = new ByteArrayContent(wizardData.Banner2.GetBytes());
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(wizardData.Banner2.ContentType);
                formData.Add(fileContent, "Files.Banner2", wizardData.Banner2.FileName);
            }

            var response = await _httpClient.PostAsync("api/property/create", formData);

            if (response.IsSuccessStatusCode)
            {
                var propertyResponse = await response.Content.ReadFromJsonAsync<CreatePropertyResponse>();
                return propertyResponse;
            }

            return null;
        }
        catch
        {
            return null;
        }
    }

    public async Task<List<PropertyListItemResponse>> GetPropertiesByGroupIdAsync(Guid groupId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/property/bygroup/{groupId}");
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var properties = System.Text.Json.JsonSerializer.Deserialize<List<PropertyListItemResponse>>(
                    content,
                    new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return properties ?? new List<PropertyListItemResponse>();
            }

            return new List<PropertyListItemResponse>();
        }
        catch
        {
            return new List<PropertyListItemResponse>();
        }
    }
}
