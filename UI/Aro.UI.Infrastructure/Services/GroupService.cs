using Aro.UI.Application.DTOs.Group;
using System.Net.Http.Json;


namespace Aro.UI.Infrastructure.Services;

public class GroupService(HttpClient httpClient) : IGroupService
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task<CreateGroupResponse?> CreateGroup(CreateGroupRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/group/create", request);
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
