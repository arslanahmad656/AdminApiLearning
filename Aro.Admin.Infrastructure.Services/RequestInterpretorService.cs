using Aro.Admin.Application.Services;
using Microsoft.AspNetCore.Http;

namespace Aro.Admin.Infrastructure.Services;

public class RequestInterpretorService(IHttpContextAccessor httpContextAccessor) : IRequestInterpretorService
{
    private readonly HttpContext? httpContext = httpContextAccessor.HttpContext;

    public string? ExtractUsername() => httpContext?.User?.Identity?.Name;

    public string? RetrieveIpAddress()
    {
        // Check for forwarded headers first (useful when behind proxy/load balancer)
        var forwardedHeader = httpContext?.Request?.Headers?["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedHeader))
        {
            // May contain multiple IPs, take the first one
            return forwardedHeader.Split(',')[0];
        }

        // Fallback to remote IP address
        return httpContext?.Connection?.RemoteIpAddress?.ToString();
    }
}
