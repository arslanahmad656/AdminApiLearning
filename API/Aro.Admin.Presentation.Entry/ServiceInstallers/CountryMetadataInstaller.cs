using Aro.Common.Application.Shared.Metadata;

namespace Aro.Admin.Presentation.Entry.ServiceInstallers;

internal class CountryMetadataInstaller : IServiceInstaller
{
    public void Install(WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<ICountryMetadataProvider, CountryMetadataProvider>();
    }
}
