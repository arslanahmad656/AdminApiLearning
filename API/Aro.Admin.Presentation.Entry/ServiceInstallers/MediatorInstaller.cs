
using Aro.Admin.Application.Mediator;

namespace Aro.Admin.Presentation.Entry.ServiceInstallers;

internal class MediatorInstaller : IServiceInstaller
{
    public void Install(WebApplicationBuilder builder)
    {
        builder.Services.AddMediatR(conf =>
        {
            conf.RegisterServicesFromAssembly(typeof(Common.Application.Mediator.AssemblyReference).Assembly);
            conf.RegisterServicesFromAssembly(typeof(Admin.Application.Mediator.AssemblyReference).Assembly);
            conf.RegisterServicesFromAssembly(typeof(Booking.Application.Mediator.AssemblyReference).Assembly);
        });
    }
}
