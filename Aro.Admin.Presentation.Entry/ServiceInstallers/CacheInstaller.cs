
namespace Aro.Admin.Presentation.Entry.ServiceInstallers;

public class CacheInstaller : IServiceInstaller
{
    public void Install(WebApplicationBuilder builder)
    {
        builder.Services.AddDistributedMemoryCache();
    }
}
