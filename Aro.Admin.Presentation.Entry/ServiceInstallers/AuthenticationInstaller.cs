using Aro.Admin.Application.Services.AccessToken;
using Aro.Admin.Application.Services.Authentication;
using Aro.Admin.Application.Shared.Options;
using Aro.Admin.Infrastructure.Services;
using Aro.Common.Domain.Shared;
using Aro.Common.Domain.Shared.Exceptions;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Aro.Admin.Presentation.Entry.ServiceInstallers;

public class AuthenticationInstaller : IServiceInstaller
{
    public void Install(WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IAccessTokenService, JwtTokenService>();
        builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        builder.Services.AddScoped<ITokenBlackListService, CacheBasedTokenBlackListService>();
        builder.Services.AddScoped<IActiveAccessTokenService, CacheBasedActiveAccessTokensService>();
        builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

        var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>()
            ?? throw new AroException(new ErrorCodes().CONFIGURATION_ERROR, $"Could not load the jwt options from the app settings. Make sure that JWT section is populated there.");

        builder.Services
            .AddAuthentication("Bearer")
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = jwtOptions.ValidateIssuer,
                    ValidateAudience = jwtOptions.ValidateAudience,
                    ValidateLifetime = jwtOptions.ValidateLifetime,
                    ValidateIssuerSigningKey = jwtOptions.ValidateIssuerSigningKey,

                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtOptions.Key)
                    ),
                    ClockSkew = TimeSpan.Zero
                };

                options.Events ??= new();
                options.Events.OnTokenValidated = async context =>
                {
                    var jti = context.Principal?.FindFirst(JwtRegisteredClaimNames.Jti);
                    if (jti is not null)
                    {
                        var blackListService = context.HttpContext.RequestServices.GetRequiredService<ITokenBlackListService>();
                        var isBlackListed = await blackListService.IsBlackListed(jti.Value, context.HttpContext.RequestAborted).ConfigureAwait(false);

                        if (isBlackListed)
                        {
                            context.Fail($"Token has been expired.");
                        }
                    }
                };
            });
    }
}
