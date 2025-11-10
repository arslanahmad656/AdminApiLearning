using Aro.Common.Application.Services.LogManager;
using Aro.Common.Application.Services.RequestInterpretor;
using Aro.Common.Domain.Shared;
using Aro.Common.Domain.Shared.Exceptions;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Aro.Common.Infrastructure.Services;

public class RequestInterpretorService(IHttpContextAccessor httpContextAccessor, ILogManager<RequestInterpretorService> logger, ErrorCodes errorCodes) : IRequestInterpretorService
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

    public Guid? GetCurrentUserId()
    {
        logger.LogDebug("Starting {MethodName}", nameof(GetCurrentUserId));

        var idClaimValue = httpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        Guid? userId = Guid.TryParse(idClaimValue, out var x) ? x : null;
        logger.LogDebug("Current user ID retrieved: {UserId}", userId ?? default);

        logger.LogDebug("Completed {MethodName}", nameof(GetCurrentUserId));
        return userId;
    }

    public TokenInfo GetTokenInfo()
    {
        logger.LogDebug("Starting {MethodName}", nameof(GetTokenInfo));

        var user = httpContext?.User ?? throw new AroUnauthorizedException(errorCodes.TOKEN_INFO_RETRIEVAL_ERROR, $"Cannot extract the token information out of an unauthorized context.");
        logger.LogDebug("User claims retrieved from HTTP context");

        var jti = user.FindFirst(JwtRegisteredClaimNames.Jti);
        var expUnix = user.FindFirst(JwtRegisteredClaimNames.Exp);
        logger.LogDebug("JWT claims extracted, jti: {Jti}, exp: {Exp}",
            jti?.Value ?? throw new AroInvalidOperationException(errorCodes.TOKEN_INFO_RETRIEVAL_ERROR, $"Could not find {JwtRegisteredClaimNames.Jti} value in the claims."),
            expUnix?.Value ?? throw new AroInvalidOperationException(errorCodes.TOKEN_INFO_RETRIEVAL_ERROR, $"Could not find {JwtRegisteredClaimNames.Exp} value in the claims."));

        var expSeconds = long.Parse(expUnix.Value);
        var expiry = DateTimeOffset.FromUnixTimeSeconds(expSeconds).UtcDateTime;
        logger.LogDebug("Token expiry calculated: {Expiry}", expiry);

        var tokenInfo = new TokenInfo(jti.Value, expiry);
        logger.LogDebug("Token info created successfully, jti: {Jti}, expiry: {Expiry}", tokenInfo.TokenIdentifier, tokenInfo.Expiry);

        logger.LogDebug("Completed {MethodName}", nameof(GetTokenInfo));
        return tokenInfo;
    }

    public bool IsAuthenticated()
    {
        logger.LogDebug("Starting {MethodName}", nameof(IsAuthenticated));

        var isAuthenticated = httpContext?.User?.Identity?.IsAuthenticated ?? false;
        logger.LogDebug("Authentication status: {IsAuthenticated}", isAuthenticated);

        logger.LogDebug("Completed {MethodName}", nameof(IsAuthenticated));
        return isAuthenticated;
    }
}
