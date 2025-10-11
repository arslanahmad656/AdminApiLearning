using Aro.Admin.Application.Services;
using Aro.Admin.Application.Services.DataServices;
using Aro.Admin.Application.Services.DTOs.ServiceResponses;
using Aro.Admin.Application.Shared.Options;
using Aro.Admin.Domain.Shared;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Aro.Admin.Infrastructure.Services;

public class JwtTokenService(IUserService userService, JwtOptions jwtOptions, SharedKeys sharedKeys) : IAccessTokenService
{
    public async Task<AccessTokenResponse> GenerateAccessToken(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await userService.GetUserById(userId, true, true, cancellationToken).ConfigureAwait(false);

        var claims = new List<Claim>
        {
            new(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.Name, user.DisplayName),
            new(ClaimTypes.Email, user.Email),
            new(sharedKeys.JWT_CLAIM_ACTIVE, user.IsActive.ToString())
        };

        claims.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, role.Id.ToString())));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expiry = DateTime.UtcNow.AddMinutes(jwtOptions.AccessTokenExpirationMinutes);
        var token = new JwtSecurityToken
        (
            issuer: jwtOptions.Issuer,
            audience: jwtOptions.Audience,
            claims: claims,
            signingCredentials: creds,
            expires: expiry
        );

        var serializedToken = new JwtSecurityTokenHandler().WriteToken(token);

        return new(serializedToken, expiry);
    }
}
