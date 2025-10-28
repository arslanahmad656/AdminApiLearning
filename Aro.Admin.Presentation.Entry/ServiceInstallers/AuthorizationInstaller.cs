
using Aro.Admin.Application.Services;
using Aro.Admin.Application.Services.SystemContext;
using Aro.Admin.Infrastructure.Services;
using Aro.Admin.Infrastructure.Services.SystemContext;

namespace Aro.Admin.Presentation.Entry.ServiceInstallers;

public class AuthorizationInstaller : IServiceInstaller
{
    public void Install(WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IAuthorizationService, AuthorizationService>();
        builder.Services.AddScoped<ISystemContext, SystemContext>();
        builder.Services.AddScoped<IPasswordResetTokenService, PasswordResetTokenService>();
        builder.Services.AddScoped<IPasswordResetLinkService, PasswordResetLinkGenerationService>();
        builder.Services.AddAuthorization();
    }
}
