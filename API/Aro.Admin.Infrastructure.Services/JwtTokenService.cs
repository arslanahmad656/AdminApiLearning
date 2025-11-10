using Aro.Admin.Application.Services.AccessToken;
using Aro.Admin.Application.Services.User;
using Aro.Admin.Application.Shared.Options;
using Aro.Admin.Domain.Shared;
using Aro.Common.Application.Services.LogManager;
using Aro.Common.Application.Services.UniqueIdGenerator;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Aro.Admin.Infrastructure.Services;

public class JwtTokenService(IUserService userService, IUniqueIdGenerator idGenerator, IActiveAccessTokenService activeAccessTokenService, IOptions<JwtOptions> jwtOptions, SharedKeys sharedKeys, ILogManager<JwtTokenService> logger) : IAccessTokenService
{
    private readonly JwtOptions jwtOptions = jwtOptions.Value;

    public async Task<AccessTokenResponse> GenerateAccessToken(Guid userId, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Starting {MethodName}", nameof(GenerateAccessToken));
        
        logger.LogDebug("Retrieving user information for token generation: {UserId}", userId);
        var user = await userService.GetUserById(userId, true, true, cancellationToken).ConfigureAwait(false);
        logger.LogDebug("User retrieved for token generation: {UserId}, email: {Email}", userId, user.Email);

        var jti = idGenerator.Generate().ToString();
        logger.LogDebug("Generated JTI for token: {Jti}", jti);
        
        var claims = new List<Claim>
        {
            new(type: JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.Name, user.DisplayName),
            new(ClaimTypes.Email, user.Email),
            new(sharedKeys.JWT_CLAIM_ACTIVE, user.IsActive.ToString()),
            new(JwtRegisteredClaimNames.Jti, jti),
        };
        logger.LogDebug("Created base claims for user: {UserId}, claimCount: {ClaimCount}", userId, claims.Count);

        claims.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, role.Id.ToString())));
        logger.LogDebug("Added role claims for user: {UserId}, totalClaimCount: {TotalClaimCount}", userId, claims.Count);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        logger.LogDebug("Created signing credentials for user: {UserId}", userId);

        var expiry = DateTime.UtcNow.AddMinutes(jwtOptions.AccessTokenExpirationMinutes);
        logger.LogDebug("Calculated token expiry for user: {UserId}, expiry: {Expiry}", userId, expiry);
        
        var token = new JwtSecurityToken
        (
            issuer: jwtOptions.Issuer,
            audience: jwtOptions.Audience,
            claims: claims,
            signingCredentials: creds,
            expires: expiry
        );
        logger.LogDebug("Created JWT token for user: {UserId}, issuer: {Issuer}, audience: {Audience}", userId, jwtOptions.Issuer, jwtOptions.Audience);

        var serializedToken = new JwtSecurityTokenHandler().WriteToken(token);
        logger.LogDebug("Serialized JWT token for user: {UserId}, tokenLength: {TokenLength}", userId, serializedToken.Length);

        logger.LogDebug("Registering token with active access token service for user: {UserId}", userId);
        await activeAccessTokenService.RegisterToken(user.Id, jti, DateTime.Now.AddMinutes(jwtOptions.AccessTokenExpirationMinutes), cancellationToken).ConfigureAwait(false);
        logger.LogDebug("Token registered with active access token service for user: {UserId}", userId);

        var response = new AccessTokenResponse(serializedToken, expiry, jti);
        logger.LogInfo("Access token generated successfully for user: {UserId}, jti: {Jti}, expiry: {Expiry}", userId, jti, expiry);
        
        logger.LogDebug("Completed {MethodName}", nameof(GenerateAccessToken));
        return response;
    }
}
