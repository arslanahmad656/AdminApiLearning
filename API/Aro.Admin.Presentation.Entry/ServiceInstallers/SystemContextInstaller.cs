using Aro.Common.Application.Services.SystemContext;
using Aro.Common.Infrastructure.Services.SystemContext;

namespace Aro.Admin.Presentation.Entry.ServiceInstallers;

public class SystemContextInstaller : IServiceInstaller
{
    public void Install(WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<SystemContextState>();
        builder.Services.AddScoped<ISystemContext>(sp => sp.GetRequiredService<SystemContextState>());
        builder.Services.AddScoped<ISystemContextEnabler, SystemContextEnabler>();
        builder.Services.AddScoped<ISystemContextFactory, SystemContextFactory>();

    }
}
