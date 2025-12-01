using Aro.Common.Application.Services.FileResource;
using Aro.Common.Infrastructure.Services.Azure.FileManager.Extensions;
using Aro.Common.Infrastructure.Services.FileManager.Extensions;
using Aro.Common.Infrastructure.Services.FileManager.Options;
using Aro.Common.Infrastructure.Services.FileResource;

namespace Aro.Admin.Presentation.Entry.ServiceInstallers;

public class FileServiceInstaller : IServiceInstaller
{
    public void Install(WebApplicationBuilder builder)
    {
#if DEBUG
        builder.Services.AddLocalFileManagement(builder.Configuration); // Cut the overhead of Blob when in development mode.
        
#else
        builder.Services.AddBlobFileManagement(builder.Configuration);
#endif

        builder.Services.AddScoped<IFileResourceService, FileResourceService>();
    }
}
