using Aro.Common.Application.Services.FileManager;
using Aro.Common.Application.Services.LogManager;
using Aro.Common.Infrastructure.Services.FileManager.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Aro.Common.Infrastructure.Services.FileManager.Extensions;

public static class LocalFileManagementExtensions
{
    public static IServiceCollection AddLocalFileManagement(this IServiceCollection services, IConfiguration configuration)
    {
        var storageOptions = configuration
                                       .GetSection("LocalStorage")
                                       .Get<LocalStorageOptions>()
                                       ?? throw new InvalidOperationException("Could not get the app settings against LocalStorage section.");
        services.AddSingleton<IFileManager>(sp => new LocalFileManager(storageOptions.Path, string.Empty, string.Empty, sp.GetRequiredService<ILogManager<LocalFileManager>>()));
        return services;
    }
}
