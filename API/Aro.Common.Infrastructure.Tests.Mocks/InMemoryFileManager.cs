using Aro.Common.Application.Services.FileManager;
using Aro.Common.Application.Services.LogManager;
using Aro.Common.Domain.Shared.Exceptions;

namespace Aro.Common.Infrastructure.Tests.Mocks;

public partial class InMemoryFileManager(string storage, string area, string? root, ILogManager<InMemoryFileManager> logger) : IFileManager
{
    private readonly Dictionary<string, byte[]> Store = new();
    private readonly string Base = BuildBase(storage, area, root);

    public Task<string> CreateFileAsync(string fileName, Stream content, string? root = null)
    {
        logger.LogInfo("Starting in-memory file creation. FileName: {FileName}, SubFolder: {SubFolder}", fileName, root ?? string.Empty);
        var key = BuildKey(fileName, root);
        logger.LogDebug("Built file key: {Key}", key);

        if (Store.ContainsKey(key))
        {
            logger.LogWarn("Mock file creation failed - file already exists. Key: {Key}", key);
            throw new AroFileManagementException(
                AroFileManagementErrorCode.FILE_CREATION_FAILED,
                $"Mock file '{key}' already exists.");
        }

        logger.LogDebug("Copying content to memory stream. Key: {Key}", key);
        using var ms = new MemoryStream();
        content.CopyTo(ms);
        Store[key] = ms.ToArray();
        logger.LogInfo("Successfully created in-memory file. Key: {Key}, Size: {Size} bytes", key, ms.Length);

        return Task.FromResult(key);
    }

    public Task<Stream> ReadFileAsync(string fileName, string? root = null)
    {
        logger.LogInfo("Starting in-memory file read. FileName: {FileName}, SubFolder: {SubFolder}", fileName, root ?? string.Empty);
        var key = BuildKey(fileName, root);
        logger.LogDebug("Built file key: {Key}", key);

        if (!Store.ContainsKey(key))
        {
            logger.LogWarn("Mock file read failed - file not found. Key: {Key}", key);
            throw new AroFileManagementException(
                AroFileManagementErrorCode.FILE_NOT_FOUND,
                $"Mock file '{key}' does not exist.");
        }

        logger.LogDebug("Retrieving file from memory store. Key: {Key}, Size: {Size} bytes", key, Store[key].Length);
        logger.LogInfo("Successfully retrieved in-memory file. Key: {Key}", key);
        return Task.FromResult<Stream>(new MemoryStream(Store[key]));
    }

    public Task<string> UpdateFileAsync(string fileName, Stream content, string? root = null)
    {
        logger.LogInfo("Starting in-memory file update. FileName: {FileName}, SubFolder: {SubFolder}", fileName, root ?? string.Empty);
        var key = BuildKey(fileName, root);
        logger.LogDebug("Built file key: {Key}", key);

        if (!Store.ContainsKey(key))
        {
            logger.LogWarn("Mock file update failed - file not found. Key: {Key}", key);
            throw new AroFileManagementException(
                AroFileManagementErrorCode.FILE_NOT_FOUND,
                $"Mock file '{key}' not found.");
        }

        logger.LogDebug("Copying updated content to memory stream. Key: {Key}", key);
        using var ms = new MemoryStream();
        content.CopyTo(ms);
        Store[key] = ms.ToArray();
        logger.LogInfo("Successfully updated in-memory file. Key: {Key}, Size: {Size} bytes", key, ms.Length);

        return Task.FromResult(key);
    }

    public Task<bool> DeleteFileAsync(string fileName, string? root = null)
    {
        logger.LogInfo("Starting in-memory file deletion. FileName: {FileName}, SubFolder: {SubFolder}", fileName, root ?? string.Empty);
        var key = BuildKey(fileName, root);
        logger.LogDebug("Built file key: {Key}", key);

        if (!Store.Remove(key))
        {
            logger.LogWarn("Mock file deletion failed - file not found. Key: {Key}", key);
            throw new AroFileManagementException(
                AroFileManagementErrorCode.FILE_NOT_FOUND,
                $"Mock file '{key}' not found.");
        }

        logger.LogInfo("Successfully deleted in-memory file. Key: {Key}", key);
        return Task.FromResult(true);
    }

    public string GetFileUrl(string fileName, string? root = null)
    {
        logger.LogDebug("Getting file URL. FileName: {FileName}, SubFolder: {SubFolder}", fileName, root ?? string.Empty);
        var url = BuildKey(fileName, root);
        logger.LogDebug("Generated file URL: {Url}", url);
        return url;
    }
}
