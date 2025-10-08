
using Aro.Admin.Application.Shared.Options;

namespace Aro.Admin.Presentation.Entry.ServiceInstallers;

internal class OptionsInstaller : IServiceInstaller
{
    public void Install(WebApplicationBuilder builder)
    {
        builder.Services.Configure<AdminSettings>(builder.Configuration.GetSection("AdminSettings"));
        builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
    }
}
