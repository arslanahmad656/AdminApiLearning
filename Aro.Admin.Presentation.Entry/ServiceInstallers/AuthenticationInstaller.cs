
using Aro.Admin.Application.Services;
using Aro.Admin.Application.Shared.Options;
using Aro.Admin.Domain.Shared.Exceptions;
using Aro.Admin.Infrastructure.Services;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Aro.Admin.Presentation.Entry.ServiceInstallers;

public class AuthenticationInstaller : IServiceInstaller
{
    public void Install(WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<ICurrentUserService, HttpContextBasedCurrentUserService>();
        builder.Services.AddScoped<IAccessTokenService, JwtTokenService>();
        builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();
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
            });
    }
}
