using Aro.Common.Application.Services.Country;
using Aro.Common.Application.Services.LogManager;
using Aro.Common.Application.Services.Serializer;
using Aro.Common.Domain.Shared;
using Aro.Common.Infrastructure.Services;

namespace Aro.Admin.Presentation.Entry.ServiceInstallers;

internal class CountryMetadataInstaller : IServiceInstaller
{
    public void Install(WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<ICountryService, CountryService>();
    }
}

