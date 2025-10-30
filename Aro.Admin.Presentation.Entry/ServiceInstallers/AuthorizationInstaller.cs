
using Aro.Admin.Application.Services;
using Aro.Admin.Infrastructure.Services;

namespace Aro.Admin.Presentation.Entry.ServiceInstallers;

public class AuthorizationInstaller : IServiceInstaller
{
    public void Install(WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IAuthorizationService, AuthorizationService>();
        builder.Services.AddScoped<IPasswordResetTokenService, PasswordResetTokenService>();
        builder.Services.AddScoped<IPasswordResetLinkService, PasswordResetLinkGenerationService>();
        builder.Services.AddScoped<IPasswordHistoryEnforcer, PasswordHistoryEnforcer>();
        builder.Services.AddTransient<IPasswordComplexityService, EasyPasswordValidator>(); // must be transient due to internal state
        builder.Services.AddAuthorization();
    }
}
