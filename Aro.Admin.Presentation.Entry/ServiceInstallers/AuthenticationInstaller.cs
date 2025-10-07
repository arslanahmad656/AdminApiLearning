
using Aro.Admin.Application.Services;
using Aro.Admin.Infrastructure.Services;

namespace Aro.Admin.Presentation.Entry.ServiceInstallers;

public class AuthenticationInstaller : IServiceInstaller
{
    public void Install(WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<ICurrentUserService, HttpContextBasedCurrentUserService>();
    }
}
