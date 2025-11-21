using Aro.Common.Application.Services.FileManager;
using Microsoft.Extensions.DependencyInjection;

namespace Aro.Common.Infrastructure.Services.FileManager.Extensions;

public static class LocalFileManagementExtensions
{
    public static IServiceCollection AddLocalFileManagement(this IServiceCollection services)
    {
        services.AddSingleton<IFileManagerFactory, LocalFileManagerFactory>();
        return services;
    }
}
