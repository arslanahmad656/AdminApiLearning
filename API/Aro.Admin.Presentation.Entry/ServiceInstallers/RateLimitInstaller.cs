using AspNetCoreRateLimit;

namespace Aro.Admin.Presentation.Entry.ServiceInstallers;

internal class RateLimitInstaller : IServiceInstaller
{
    public void Install(WebApplicationBuilder builder)
    {
        // Load rate limiting configuration from appsettings
        builder.Services.Configure<IpRateLimitOptions>(
            builder.Configuration.GetSection("IpRateLimiting"));
        builder.Services.Configure<IpRateLimitPolicies>(
            builder.Configuration.GetSection("IpRateLimitPolicies"));

        // Add memory cache for rate limiting (required)
        builder.Services.AddMemoryCache();

        // Add in-memory rate limiting stores
        builder.Services.AddInMemoryRateLimiting();

        // Add rate limit configuration
        builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
    }
}
