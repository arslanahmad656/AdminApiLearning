using Aro.Admin.Application.Services;
using Aro.Admin.Application.Services.DTOs.ServiceResponses;
using Aro.Admin.Domain.Shared.Exceptions;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;

namespace Aro.Admin.Infrastructure.Services;

public class HttpContextBasedCurrentUserService(IHttpContextAccessor contextAccessor, ErrorCodes errorCodes, ILogManager<HttpContextBasedCurrentUserService> logger) : ICurrentUserService
{
    private readonly HttpContext? httpContext = contextAccessor.HttpContext;
    
    public Guid? GetCurrentUserId()
    {
        logger.LogDebug("Starting {MethodName}", nameof(GetCurrentUserId));
        
        var userId = Guid.TryParse(httpContext?.User?.Identity?.Name, out var parsedUserId) ? parsedUserId : null;
        logger.LogDebug("Current user ID retrieved: {UserId}", userId);
        
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
        logger.LogDebug("JWT claims extracted, jti: {Jti}, exp: {Exp}", jti?.Value, expUnix?.Value);

        var expSeconds = long.Parse(expUnix?.Value ?? throw new AroInvalidOperationException(errorCodes.TOKEN_INFO_RETRIEVAL_ERROR, $"Could not find {JwtRegisteredClaimNames.Exp} value in the claims."));
        var expiry = DateTimeOffset.FromUnixTimeSeconds(expSeconds).UtcDateTime;
        logger.LogDebug("Token expiry calculated: {Expiry}", expiry);

        var tokenInfo = new TokenInfo(jti?.Value ?? throw new AroInvalidOperationException(errorCodes.TOKEN_INFO_RETRIEVAL_ERROR, $"Could not find {JwtRegisteredClaimNames.Jti} value in the claims."), expiry);
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
