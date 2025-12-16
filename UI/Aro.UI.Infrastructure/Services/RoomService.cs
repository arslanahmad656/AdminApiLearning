using Aro.UI.Application.DTOs.Room;
using System.Net.Http.Json;


namespace Aro.UI.Infrastructure.Services;

public class RoomService(HttpClient httpClient) : IRoomService
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task<CreateRoomResponse?> CreateRoom(CreateRoomRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/room/create", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<CreateRoomResponse>();
    }

    public async Task<GetRoomResponse?> GetRoom(GetRoomRequest request)
    {
        var response = await _httpClient.GetAsync($"api/room/get/{request.Id}?Include={request.Include}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<GetRoomResponse>();
    }

    public async Task<GetRoomsResponse?> GetRooms(GetRoomsRequest request)
    {
        var url = $"api/room/getall?Filter={request.Filter}&Include={request.Include}&Page={request.Page}&PageSize={request.PageSize}&SortBy={request.SortBy}&Ascending={request.Ascending}";
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<GetRoomsResponse>();
    }


    public async Task<PatchRoomResponse?> PatchRoom(PatchRoomRequest request)
    {
        var response = await _httpClient.PatchAsJsonAsync($"api/room/patch/{request.Id}", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<PatchRoomResponse>();
    }

    public async Task<DeleteRoomResponse?> DeleteRoom(Guid Id)
    {
        var response = await _httpClient.DeleteAsync($"api/room/delete/{Id}");

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<DeleteRoomResponse>();
    }
}
