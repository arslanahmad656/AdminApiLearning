using Aro.Admin.Presentation.Entry.Middleware;

namespace Aro.Admin.Presentation.Entry.Extensions;

public static partial class MiddlewareExtensions
{
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder app)
        => app.UseMiddleware<RequestLoggingMiddleware>();

    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
        => app.UseMiddleware<ExceptionHandlingMiddleware>();

    public static IApplicationBuilder EnableConfigurationSettings(this IApplicationBuilder app)
        => app.AddConfigurationSettingsEndpoint();
}
