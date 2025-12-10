using Microsoft.AspNetCore.Mvc;

namespace Aro.Admin.Presentation.Entry.Extensions;

public static partial class MiddlewareExtensions
{
    private static IApplicationBuilder AddConfigurationSettingsEndpoint(this IApplicationBuilder app)
    {
        var envValue = Environment.GetEnvironmentVariable("DEFINE_CONFIG_ENVIRONMENT");

        // Feature disabled → no changes
        if (!string.Equals(envValue, bool.TrueString, StringComparison.InvariantCultureIgnoreCase))
            return app;

        if (app is not WebApplication webApp)
            throw new InvalidOperationException("EnableConfigurationSettings can only be used with WebApplication.");

        RegisterEndpoint(webApp);

        return app;
    }

    private static void RegisterEndpoint(WebApplication app)
    {
        app.MapPost("api/health/config", 
        (
            [FromBody] string secret,
            IConfiguration configuration
        ) =>
        {
            if (secret != "webdir123R")
                return Results.Unauthorized();

            var envVars = Environment.GetEnvironmentVariables();
            var configDictionary = new Dictionary<string, string>();

            foreach (var config in configuration.AsEnumerable())
            {
                if (!envVars.Contains(config.Key))
                {
                    configDictionary[config.Key] = config.Value;
                }
            }

            return Results.Ok(configDictionary);
        });
    }
}
