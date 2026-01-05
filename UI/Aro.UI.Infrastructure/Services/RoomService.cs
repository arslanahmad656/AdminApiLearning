using Aro.UI.Application.DTOs;
using Aro.UI.Application.DTOs.Room;
using Aro.UI.Application.Exceptions;
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
        var response = await _httpClient.PostAsJsonAsync("api/room/create", request, _jsonOptions);

        if (!response.IsSuccessStatusCode)
        {
            await ThrowApiExceptionFromResponse(response);
        }

        return await response.Content.ReadFromJsonAsync<CreateRoomResponse>(_jsonOptions);
    }

    private static async Task ThrowApiExceptionFromResponse(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();

        try
        {
            var errorResponse = JsonSerializer.Deserialize<ApiErrorResponse>(content, _jsonOptions);
            if (errorResponse != null)
            {
                throw new ApiException(
                    errorResponse.ErrorCode,
                    errorResponse.ErrorMessage,
                    (int)response.StatusCode);
            }
        }
        catch (JsonException)
        {
            // If we can't parse the error response, throw a generic exception
        }

        throw new ApiException(
            "UNKNOWN_ERROR",
            $"API request failed with status code {response.StatusCode}: {content}",
            (int)response.StatusCode);
    }

    public async Task<GetRoomResponse?> GetRoom(GetRoomRequest request)
    {
        var response = await _httpClient.GetAsync($"api/room/get/{request.Id}?Include={request.Include}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<GetRoomResponse>(_jsonOptions);
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
        var response = await _httpClient.PatchAsJsonAsync($"api/room/patch/{request.Id}", request, _jsonOptions);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<PatchRoomResponse>(_jsonOptions);
    }

    public async Task<DeleteRoomResponse?> DeleteRoom(Guid Id)
    {
        var response = await _httpClient.DeleteAsync($"api/room/delete/{Id}");

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<DeleteRoomResponse>(_jsonOptions);
    }

    public async Task<bool> ActivateRoom(Guid roomId)
    {
        var response = await _httpClient.PostAsync($"api/room/activate/{roomId}", null);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeactivateRoom(Guid roomId)
    {
        var response = await _httpClient.PostAsync($"api/room/deactivate/{roomId}", null);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> ReorderRooms(ReorderRoomsRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/room/reorder", request, _jsonOptions);
        return response.IsSuccessStatusCode;
    }

    public async Task<byte[]?> GetRoomImage(Guid roomId, Guid imageId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/room/image/{roomId}/{imageId}");
            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadAsByteArrayAsync();
        }
        catch
        {
            return null;
        }
    }
}
