using Aro.Common.Infrastructure.Services.Azure.FileManager.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Aro.Common.Infrastructure.DependencyInjection;

public static class CommonServicesExtensions
{
    public static IServiceCollection AddCommonServices(this IServiceCollection services)
    {
        services.AddBlobFileManagement();
        return services;
    }
}
