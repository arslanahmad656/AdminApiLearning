using Aro.Admin.Presentation.UI;
using Aro.Admin.Presentation.UI.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configure API base address (update this to match your API URL)
var apiBaseAddress = builder.Configuration["ApiBaseAddress"] ?? "https://localhost:7225/";

// Register HttpClient with authorization handler
builder.Services.AddScoped<AuthorizationMessageHandler>();

builder.Services.AddHttpClient("AroAdminAPI", client =>
{
    client.BaseAddress = new Uri(apiBaseAddress);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
})
.AddHttpMessageHandler<AuthorizationMessageHandler>();

// Register default HttpClient for backward compatibility
builder.Services.AddScoped(sp =>
{
    var clientFactory = sp.GetRequiredService<IHttpClientFactory>();
    return clientFactory.CreateClient("AroAdminAPI");
});

// Register authentication services
builder.Services.AddScoped<ITokenStorageService, TokenStorageService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

// Register custom AuthenticationStateProvider
builder.Services.AddScoped<ApiAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider =>
    provider.GetRequiredService<ApiAuthenticationStateProvider>());

// Add authorization
builder.Services.AddAuthorizationCore();

// Add MudBlazor
builder.Services.AddMudServices();

await builder.Build().RunAsync();
