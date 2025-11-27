using Aro.Common.Application.Services.FileManager;
using Aro.Common.Application.Services.LogManager;
using Azure.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Aro.Common.Infrastructure.Services.Azure.FileManager;

public class BlobFileManagerFactory(TokenCredential credential, ILogManager<BlobFileManagerFactory> logger, IServiceProvider serviceProvider) : IFileManagerFactory
{
    public IFileManager Create(string storage, string area, string? root = null)
    {
        logger.LogInfo("Creating BlobFileManager instance. Storage: {Storage}, Area: {Area}, Root: {Root}", storage, area, root ?? string.Empty);
        var fileManagerLogger = serviceProvider.GetRequiredService<ILogManager<BlobFileManager>>();
        var fileManager = new BlobFileManager(storage, area, root, credential, fileManagerLogger);
        logger.LogDebug("BlobFileManager instance created successfully");
        return fileManager;
    }
}
