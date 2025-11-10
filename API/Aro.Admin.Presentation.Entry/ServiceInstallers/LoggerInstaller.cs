using Aro.Common.Application.Services.LogManager;
using Serilog;

namespace Aro.Admin.Presentation.Entry.ServiceInstallers;

public class LoggerInstaller : IServiceInstaller
{
    public void Install(WebApplicationBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .Enrich.FromLogContext()
            .CreateLogger();

        builder.Host.UseSerilog();

        builder.Services.AddSingleton<ILogManager, Infrastructure.Services.SeriLogger>();
        builder.Services.AddSingleton(typeof(ILogManager<>), typeof(Infrastructure.Services.SeriLogger<>));
    }
}
