using Aro.Admin.Application.Services;
using Aro.Admin.Application.Services.DTOs.ServiceResponses;
using Aro.Admin.Domain.Shared.Exceptions;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;

namespace Aro.Admin.Infrastructure.Services;

public class HttpContextBasedCurrentUserService(IHttpContextAccessor contextAccessor, ErrorCodes errorCodes) : ICurrentUserService
{
    private readonly HttpContext? httpContext = contextAccessor.HttpContext;
    
    public Guid? GetCurrentUserId() => Guid.TryParse(httpContext?.User?.Identity?.Name, out var userId) ? userId : null;

    public TokenInfo GetTokenInfo()
    {
        var user = httpContext?.User ?? throw new AroUnauthorizedException(errorCodes.TOKEN_INFO_RETRIEVAL_ERROR, $"Cannot extract the token information out of an unauthorized context.");

        var jti = user.FindFirst(JwtRegisteredClaimNames.Jti);
        var expUnix = user.FindFirst(JwtRegisteredClaimNames.Exp);

        var expSeconds = long.Parse(expUnix?.Value ?? throw new AroInvalidOperationException(errorCodes.TOKEN_INFO_RETRIEVAL_ERROR, $"Could not find {JwtRegisteredClaimNames.Exp} value in the claims."));
        var expiry = DateTimeOffset.FromUnixTimeSeconds(expSeconds).UtcDateTime;

        var tokenInfo = new TokenInfo(jti?.Value ?? throw new AroInvalidOperationException(errorCodes.TOKEN_INFO_RETRIEVAL_ERROR, $"Could not find {JwtRegisteredClaimNames.Jti} value in the claims."), expiry);

        return tokenInfo;
    }

    public bool IsAuthenticated() => httpContext?.User?.Identity?.IsAuthenticated ?? false;
}
