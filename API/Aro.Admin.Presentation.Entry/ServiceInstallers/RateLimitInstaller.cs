using AspNetCoreRateLimit;

namespace Aro.Admin.Presentation.Entry.ServiceInstallers;

internal class RateLimitInstaller : IServiceInstaller
{
    public void Install(WebApplicationBuilder builder)
    {
        builder.Services.Configure<IpRateLimitOptions>(
            builder.Configuration.GetSection("IpRateLimiting"));
        builder.Services.Configure<IpRateLimitPolicies>(
            builder.Configuration.GetSection("IpRateLimitPolicies"));

        builder.Services.AddMemoryCache();

        builder.Services.AddInMemoryRateLimiting();

        builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
    }
}
