using Aro.Common.Application.Services.FileManager;
using Aro.Common.Application.Services.LogManager;
using Microsoft.Extensions.DependencyInjection;

namespace Aro.Common.Infrastructure.Services.FileManager;

public class LocalFileManagerFactory(ILogManager<LocalFileManagerFactory> logger, IServiceProvider serviceProvider) : IFileManagerFactory
{
    public IFileManager Create(string storage, string area, string? root = null)
    {
        logger.LogInfo("Creating LocalFileManager instance. Storage: {Storage}, Area: {Area}, Root: {Root}", storage, area, root ?? string.Empty);
        var fileManagerLogger = serviceProvider.GetRequiredService<ILogManager<LocalFileManager>>();
        var fileManager = new LocalFileManager(storage, area, root, fileManagerLogger);
        logger.LogDebug("LocalFileManager instance created successfully");
        return fileManager;
    }
}

