using System.Text.Json.Serialization;

namespace Aro.Admin.Presentation.Entry.ServiceInstallers;

internal class ControllerInstaller : IServiceInstaller
{
    public void Install(WebApplicationBuilder builder)
    {
        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            })
            .AddApplicationPart(typeof(Admin.Presentation.Api.AssemblyReference).Assembly)
            .AddApplicationPart(typeof(Booking.Presentation.Api.AssemblyReference).Assembly)
            .AddApplicationPart(typeof(Common.Presentation.Api.AssemblyReference).Assembly);
    }
}
