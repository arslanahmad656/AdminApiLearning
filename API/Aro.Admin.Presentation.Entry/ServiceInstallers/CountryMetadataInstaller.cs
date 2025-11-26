using Aro.Common.Application.Services.LogManager;
using Aro.Common.Application.Services.Metadata;
using Aro.Common.Application.Services.Serializer;
using Aro.Common.Domain.Shared;
using Aro.Common.Infrastructure.Services;

namespace Aro.Admin.Presentation.Entry.ServiceInstallers;

internal class CountryMetadataInstaller : IServiceInstaller
{
    public void Install(WebApplicationBuilder builder)
    {
        var metadataPath = Path.Combine(AppContext.BaseDirectory, "AppData", "CountryMetadata.json");

        builder.Services.AddSingleton<ICountryMetadataService>(sp =>
            new CountryMetadataService(
                sp.GetRequiredService<ErrorCodes>(),
                sp.GetRequiredService<ILogManager<CountryMetadataService>>(),
                sp.GetRequiredService<ISerializer>(),
                metadataPath));
    }
}

