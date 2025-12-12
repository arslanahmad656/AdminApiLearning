using System.Net.Http.Json;
using Aro.UI.Application.DTOs;


namespace Aro.UI.Infrastructure.Services;

public class GroupService(HttpClient httpClient) : IGroupService
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task<CreateGroupResponse?> CreateGroup(CreateGroupRequest request)
    {
        using var content = new MultipartFormDataContent();

        content.Add(new StringContent(request.GroupName), "GroupName");
        content.Add(new StringContent(request.AddressLine1), "AddressLine1");
        if (!string.IsNullOrEmpty(request.AddressLine2))
            content.Add(new StringContent(request.AddressLine2), "AddressLine2");
        content.Add(new StringContent(request.City), "City");
        content.Add(new StringContent(request.PostalCode), "PostalCode");
        content.Add(new StringContent(request.Country), "Country");
        content.Add(new StringContent(request.PrimaryContactId.ToString()), "PrimaryContactId");
        content.Add(new StringContent(request.IsActive.ToString().ToLower()), "IsActive");

        if (request.LogoBytes != null && request.LogoBytes.Length > 0)
        {
            var logoContent = new ByteArrayContent(request.LogoBytes);
            logoContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(
                request.LogoContentType ?? "image/png");
            content.Add(logoContent, "Logo", request.LogoFileName ?? "logo.png");
        }

        var response = await _httpClient.PostAsync("api/group/create", content);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<CreateGroupResponse>();
    }

    public async Task<GetGroupResponse?> GetGroup(GetGroupRequest request)
    {
        var response = await _httpClient.GetAsync($"api/group/get/{request.Id}?Include={request.Include}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<GetGroupResponse>();
    }

    public async Task<GetGroupsResponse?> GetGroups(GetGroupsRequest request)
    {
        var url = $"api/group/getall?Filter={request.Filter}&Include={request.Include}&Page={request.Page}&PageSize={request.PageSize}&SortBy={request.SortBy}&Ascending={request.Ascending}";
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<GetGroupsResponse>();
    }


    public async Task<PatchGroupResponse?> PatchGroup(PatchGroupRequest request)
    {
        var response = await _httpClient.PatchAsJsonAsync($"api/group/patch/{request.Id}", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<PatchGroupResponse>();
    }

    public async Task<DeleteGroupResponse?> DeleteGroup(Guid Id)
    {
        var response = await _httpClient.DeleteAsync($"api/group/delete/{Id}");

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<DeleteGroupResponse>();
    }
}
