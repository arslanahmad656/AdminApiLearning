using System.Net.Http.Json;
using Aro.UI.Application.DTOs;

namespace Aro.UI.Infrastructure.Services;

public class PolicyService(HttpClient httpClient) : IPolicyService
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task<CreatePolicyResponse?> CreatePolicy(CreatePolicyRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/policy/create", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<CreatePolicyResponse>();
    }

    public async Task<GetPolicyResponse?> GetPolicy(GetPolicyRequest request)
    {
        var url = $"api/policy/get/{request.Id}";
        if (!string.IsNullOrEmpty(request.Include))
        {
            url += $"?Include={request.Include}";
        }
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<GetPolicyResponse>();
    }

    public async Task<GetPoliciesResponse?> GetPolicies(GetPoliciesRequest request)
    {
        var url = $"api/policy/getall?Page={request.Page}&PageSize={request.PageSize}&SortBy={request.SortBy}&Ascending={request.Ascending}";

        if (request.Filter.HasValue)
        {
            url += $"&Filter={request.Filter}";
        }
        if (!string.IsNullOrEmpty(request.Include))
        {
            url += $"&Include={request.Include}";
        }

        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<GetPoliciesResponse>();
    }

    public async Task<GetPoliciesByPropertyResponse?> GetPoliciesByProperty(Guid propertyId, string? include = null)
    {
        var url = $"api/policy/getbyproperty/{propertyId}";
        if (!string.IsNullOrEmpty(include))
        {
            url += $"?Include={include}";
        }

        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<GetPoliciesByPropertyResponse>();
    }

    public async Task<PatchPolicyResponse?> PatchPolicy(PatchPolicyRequest request)
    {
        var patchBody = new
        {
            Title = request.Title,
            Description = request.Description,
            IsActive = request.IsActive
        };

        var response = await _httpClient.PatchAsJsonAsync($"api/policy/patch/{request.Id}", patchBody);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<PatchPolicyResponse>();
    }

    public async Task<DeletePolicyResponse?> DeletePolicy(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"api/policy/delete/{id}");

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<DeletePolicyResponse>();
    }

    public async Task<bool> ReorderPolicies(ReorderPoliciesRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/policy/reorder", request);
        return response.IsSuccessStatusCode;
    }
}
