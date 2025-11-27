using Aro.UI.Application.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Http;

namespace Aro.UI.Infrastructure.Services;

public class AuthorizationMessageHandler : DelegatingHandler
{
    private readonly ITokenStorageService _tokenStorage;
    private readonly NavigationManager _navigationManager;

    public AuthorizationMessageHandler(
        ITokenStorageService tokenStorage,
        NavigationManager navigationManager)
    {
        _tokenStorage = tokenStorage;
        _navigationManager = navigationManager;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        // Don't add token to auth endpoints
        var isAuthEndpoint = request.RequestUri?.AbsolutePath.Contains("/api/auth/") ?? false;

        if (!isAuthEndpoint)
        {
            var token = await _tokenStorage.GetAccessTokenAsync();

            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }

        // Enable credentials for CORS
        request.SetBrowserRequestMode(BrowserRequestMode.Cors);
        request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);

        var response = await base.SendAsync(request, cancellationToken);

        // If 401 Unauthorized, redirect to login
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            _navigationManager.NavigateTo("/login");
        }

        return response;
    }
}
