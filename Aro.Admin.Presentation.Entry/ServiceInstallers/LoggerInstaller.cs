
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

        builder.Services.AddSingleton<Application.Services.ILogManager, Infrastructure.Services.SeriLogger>();
        builder.Services.AddSingleton(typeof(Application.Services.ILogManager<>), typeof(Infrastructure.Services.SeriLogger<>));
    }
}
