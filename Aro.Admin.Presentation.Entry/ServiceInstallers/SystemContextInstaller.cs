using Aro.Admin.Application.Services;
using Aro.Admin.Application.Services.SystemContext;
using Aro.Admin.Infrastructure.Services.SystemContext;

namespace Aro.Admin.Presentation.Entry.ServiceInstallers;

public class SystemContextInstaller : IServiceInstaller
{
    public void Install(WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<ISystemContext, SystemContext>();
        builder.Services.AddScoped<ISystemContextEnabler, SystemContext>();
        builder.Services.AddScoped<ISystemContextFactory, SystemContextFactory>();
    }
}
