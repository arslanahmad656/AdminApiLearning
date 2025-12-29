using Aro.UI.Application.DTOs.Room;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace Aro.UI.Infrastructure.Services;

public class RoomService(HttpClient httpClient) : IRoomService
{
    private readonly HttpClient _httpClient = httpClient;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

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
        var url = $"api/room/getall?Page={request.Page}&PageSize={request.PageSize}&SortBy={request.SortBy}&Ascending={request.Ascending}";

        if (request.PropertyId.HasValue)
            url += $"&PropertyId={request.PropertyId.Value}";
        if (!string.IsNullOrEmpty(request.Filter))
            url += $"&Filter={request.Filter}";
        if (!string.IsNullOrEmpty(request.Include))
            url += $"&Include={request.Include}";

        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<GetRoomsResponse>(_jsonOptions);
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
