using Aro.Admin.Presentation.UI;
using Aro.Admin.Presentation.UI.Validators;
using Aro.UI.Application.DTOs.Group;
using Aro.UI.Application.DTOs.Room;
using Aro.UI.Application.Interfaces;
using Aro.UI.Infrastructure.Services;
using FluentValidation;
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
builder.Services.AddScoped<IPasswordResetService, PasswordResetService>();

// Register property services
builder.Services.AddScoped<IPropertyService, PropertyService>();

// Register error handling service
builder.Services.AddScoped<IErrorHandlingService, ErrorHandlingService>();

// Register custom AuthenticationStateProvider
builder.Services.AddScoped<ApiAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider =>
    provider.GetRequiredService<ApiAuthenticationStateProvider>());

// Add authorization
builder.Services.AddAuthorizationCore();

// Add MudBlazor with Snackbar configuration
builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = MudBlazor.Defaults.Classes.Position.TopRight;
    config.SnackbarConfiguration.PreventDuplicates = false;
    config.SnackbarConfiguration.NewestOnTop = true;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 3000; // 3 seconds
    config.SnackbarConfiguration.HideTransitionDuration = 300;
    config.SnackbarConfiguration.ShowTransitionDuration = 300;
    config.SnackbarConfiguration.SnackbarVariant = MudBlazor.Variant.Filled;
});

// Register data services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IGroupService, GroupService>();
builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<IPolicyService, PolicyService>();

builder.Services.AddScoped<ICountryMetadataService, CountryMetadataService>();

builder.Services.AddTransient<IValidator<GroupModel>, GroupModelFluentValidator>();
builder.Services.AddTransient<IValidator<PrimaryContactModel>, PrimaryContactModelFluentValidator>();

builder.Services.AddTransient<IValidator<RoomModel>, RoomModelFluentValidator>();
builder.Services.AddTransient<IValidator<Amenity>, AmenityModelFluentValidator>();

await builder.Build().RunAsync();
