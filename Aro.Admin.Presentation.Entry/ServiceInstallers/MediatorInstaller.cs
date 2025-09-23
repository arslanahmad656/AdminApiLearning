
using Aro.Admin.Application.Mediator;

namespace Aro.Admin.Presentation.Entry.ServiceInstallers;

internal class MediatorInstaller : IServiceInstaller
{
    public void Install(WebApplicationBuilder builder)
    {
        builder.Services.AddMediatR(conf =>
        {
            conf.RegisterServicesFromAssembly(typeof(AssemblyReference).Assembly);
        });
    }
}
