using Aro.Admin.Application.Services.LogManager;
using Aro.Admin.Application.Services.RequestInterpretor;
using Microsoft.AspNetCore.Http;

namespace Aro.Admin.Infrastructure.Services;

public class RequestInterpretorService(IHttpContextAccessor httpContextAccessor, ILogManager<RequestInterpretorService> logger) : IRequestInterpretorService
{
    private readonly HttpContext? httpContext = httpContextAccessor.HttpContext;

    public string? ExtractUsername()
    {
        logger.LogDebug("Starting {MethodName}", nameof(ExtractUsername));
        
        var username = httpContext?.User?.Identity?.Name;
        logger.LogDebug("Username extracted: {Username}", username ?? string.Empty);
        
        logger.LogDebug("Completed {MethodName}", nameof(ExtractUsername));
        return username;
    }

    public string? RetrieveIpAddress()
    {
        logger.LogDebug("Starting {MethodName}", nameof(RetrieveIpAddress));
        
        // Check for forwarded headers first (useful when behind proxy/load balancer)
        var forwardedHeader = httpContext?.Request?.Headers?["X-Forwarded-For"].FirstOrDefault();
        logger.LogDebug("X-Forwarded-For header: {ForwardedHeader}", forwardedHeader ?? string.Empty);
        
        if (!string.IsNullOrEmpty(forwardedHeader))
        {
            // May contain multiple IPs, take the first one
            var ipAddress = forwardedHeader.Split(',')[0];
            logger.LogDebug("IP address extracted from forwarded header: {IpAddress}", ipAddress);
            
            logger.LogDebug("Completed {MethodName}", nameof(RetrieveIpAddress));
            return ipAddress;
        }

        // Fallback to remote IP address
        var remoteIpAddress = httpContext?.Connection?.RemoteIpAddress?.ToString();
        logger.LogDebug("IP address retrieved from connection: {IpAddress}", remoteIpAddress ?? string.Empty);
        
        logger.LogDebug("Completed {MethodName}", nameof(RetrieveIpAddress));
        return remoteIpAddress;
    }

    public string? GetUserAgent()
    {
        logger.LogDebug("Starting {MethodName}", nameof(GetUserAgent));
        
        var userAgent = httpContext?.Request?.Headers?["User-Agent"].FirstOrDefault();
        logger.LogDebug("User-Agent header: {UserAgent}", userAgent ?? string.Empty);
        
        logger.LogDebug("Completed {MethodName}", nameof(GetUserAgent));
        return userAgent;
    }
}
