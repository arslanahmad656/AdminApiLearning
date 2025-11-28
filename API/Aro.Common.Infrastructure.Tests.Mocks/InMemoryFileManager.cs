using Aro.Common.Application.Services.FileManager;
using Aro.Common.Application.Services.LogManager;
using Aro.Common.Domain.Shared.Exceptions;

namespace Aro.Common.Infrastructure.Tests.Mocks;

public partial class InMemoryFileManager(string storage, string area, string? root, ILogManager<InMemoryFileManager> logger) : IFileManager
{
    private readonly Dictionary<string, byte[]> Store = new();
    private readonly string Base = BuildBase(storage, area, root);

    public Task<Uri> CreateFileAsync(string fileName, Stream content, string? root = null)
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
        var uri = new Uri(key);
        logger.LogInfo("Successfully created in-memory file. Uri: {Uri}, Size: {Size} bytes", uri, ms.Length);

        return Task.FromResult(uri);
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

    public Task<Uri> UpdateFileAsync(string fileName, Stream content, string? root = null)
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
        var uri = new Uri(key);
        logger.LogInfo("Successfully updated in-memory file. Uri: {Uri}, Size: {Size} bytes", uri, ms.Length);

        return Task.FromResult(uri);
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

    public Task<Stream> ReadFileByUriAsync(Uri uri)
    {
        logger.LogInfo("Starting in-memory file read by URI. Uri: {Uri}", uri);
        
        if (uri == null)
        {
            logger.LogWarn("Mock file read by URI failed - URI is null.");
            throw new AroFileManagementException(
                AroFileManagementErrorCode.FILE_READ_ERROR,
                "URI cannot be null.");
        }

        var key = uri.ToString();
        logger.LogDebug("Validating URI against base: {Base}", Base);

        // Validate that the URI/key starts with the configured base
        if (!key.StartsWith(Base, StringComparison.OrdinalIgnoreCase))
        {
            logger.LogWarn("Mock file read by URI failed - URI outside base. Base: {Base}, Uri: {Uri}", Base, uri);
            throw new AroFileManagementException(
                AroFileManagementErrorCode.FILE_NOT_FOUND,
                $"Mock file '{uri}' does not exist.");
        }

        if (!Store.ContainsKey(key))
        {
            logger.LogWarn("Mock file read by URI failed - file not found. Uri: {Uri}", uri);
            throw new AroFileManagementException(
                AroFileManagementErrorCode.FILE_NOT_FOUND,
                $"Mock file '{uri}' does not exist.");
        }

        logger.LogDebug("Retrieving file from memory store by URI. Uri: {Uri}, Size: {Size} bytes", uri, Store[key].Length);
        logger.LogInfo("Successfully retrieved in-memory file by URI. Uri: {Uri}", uri);
        return Task.FromResult<Stream>(new MemoryStream(Store[key]));
    }

    public Uri GetFileUrl(string fileName, string? root = null)
    {
        logger.LogDebug("Getting file URL. FileName: {FileName}, SubFolder: {SubFolder}", fileName, root ?? string.Empty);
        var key = BuildKey(fileName, root);
        var url = new Uri(key);
        logger.LogDebug("Generated file URL: {Url}", url);
        return url;
    }
}
