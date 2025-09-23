
using Aro.Admin.Presentation.Api;

namespace Aro.Admin.Presentation.Entry.ServiceInstallers;

internal class ControllerInstaller : IServiceInstaller
{
    public void Install(WebApplicationBuilder builder)
    {
        builder.Services.AddControllers()
            .AddApplicationPart(typeof(AssemblyReference).Assembly);
    }
}
