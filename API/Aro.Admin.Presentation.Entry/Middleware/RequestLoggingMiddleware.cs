namespace Aro.Admin.Presentation.Entry.Middleware;

using Aro.Admin.Application.Shared.Options;
using Aro.Common.Application.Services.LogManager;
using Microsoft.Extensions.Options;
using System.Diagnostics;

public class RequestLoggingMiddleware(RequestDelegate next, ILogManager logger, IOptions<LoggingSettings> settings)
{
    private readonly LoggingSettings logSettings = settings.Value;

    public async Task InvokeAsync(HttpContext context)
    {
        var request = context.Request;
        var stopWatch = logSettings.TrackTimeInDebugLevel ? Stopwatch.StartNew() : null;

        string body = string.Empty;
        if (logSettings.IncludeBodyInRequestLogging && request.ContentLength > 0 && request.Body.CanSeek)
        {
            request.EnableBuffering();
            using var reader = new StreamReader(request.Body, leaveOpen: true);
            body = await reader.ReadToEndAsync();
            request.Body.Position = 0;
        }

        var requestLog = new
        {
            Method = request.Method,
            Path = request.Path,
            Query = request.QueryString.ToString(),
            User = context.User.Identity?.IsAuthenticated == true ? context.User.Identity?.Name : "Anonymous",
            Body = logSettings.IncludeBodyInRequestLogging ? body : string.Empty
        };

        logger.LogDebug("Incoming request", requestLog);

        await next(context);

        stopWatch?.Stop();

        var responseLog = new
        {
            StatusCode = context.Response.StatusCode,
            Path = request.Path,
            ElapsedMilliseconds = logSettings.TrackTimeInDebugLevel ? stopWatch?.ElapsedMilliseconds : null
        };

        if (logSettings.TrackTimeInDebugLevel)
        {
            logger.LogDebug("Request completed", responseLog);
        }
        else
        {
            logger.LogDebug("Request completed", responseLog);
        }
    }
}
