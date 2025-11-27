using Aro.Common.Application.Services.FileManager;
using Aro.Common.Application.Services.LogManager;
using Aro.Common.Domain.Shared.Exceptions;

namespace Aro.Common.Infrastructure.Services.FileManager;

public partial class LocalFileManager(string storage, string area, string? root, ILogManager<LocalFileManager> logger) : IFileManager
{
    private readonly string Root = BuildRoot(storage, area, root);

    public Task<string> CreateFileAsync(string fileName, Stream content, string? root = null)
    {
        logger.LogInfo("Starting local file creation. FileName: {FileName}, SubFolder: {SubFolder}", fileName, root ?? string.Empty);
        try
        {
            var path = BuildPath(fileName, root);
            logger.LogDebug("Built file path: {Path}", path);

            if (File.Exists(path))
            {
                logger.LogWarn("File creation failed - file already exists. Path: {Path}", path);
                throw new AroFileManagementException(
                    AroFileManagementErrorCode.FILE_CREATION_FAILED,
                    $"Local file '{path}' already exists.");
            }

            logger.LogDebug("Creating new file stream for path: {Path}", path);
            using var fs = new FileStream(path, FileMode.CreateNew);
            content.CopyTo(fs);
            logger.LogInfo("Successfully created local file. Path: {Path}", path);

            return Task.FromResult(path);
        }
        catch (AroFileManagementException ex)
        {
            logger.LogError(ex, "File management exception during local file creation. FileName: {FileName}", fileName);
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error during local file creation. FileName: {FileName}", fileName);
            throw new AroFileManagementException(
                AroFileManagementErrorCode.FILE_CREATION_FAILED,
                "Local file creation failed.",
                ex);
        }
    }

    public Task<Stream> ReadFileAsync(string fileName, string? root = null)
    {
        logger.LogInfo("Starting local file read. FileName: {FileName}, SubFolder: {SubFolder}", fileName, root ?? string.Empty);
        try
        {
            var path = BuildPath(fileName, root);
            logger.LogDebug("Built file path: {Path}", path);

            if (!File.Exists(path))
            {
                logger.LogWarn("File read failed - file not found. Path: {Path}", path);
                throw new AroFileManagementException(
                    AroFileManagementErrorCode.FILE_NOT_FOUND,
                    $"Local file '{path}' not found.");
            }

            logger.LogDebug("Opening file stream for reading. Path: {Path}", path);
            Stream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            logger.LogInfo("Successfully opened local file for reading. Path: {Path}", path);
            return Task.FromResult(fs);
        }
        catch (AroFileManagementException ex)
        {
            logger.LogError(ex, "File management exception during local file read. FileName: {FileName}", fileName);
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error during local file read. FileName: {FileName}", fileName);
            throw new AroFileManagementException(
                AroFileManagementErrorCode.FILE_READ_ERROR,
                "Local read failed.",
                ex);
        }
    }

    public Task<string> UpdateFileAsync(string fileName, Stream content, string? root = null)
    {
        logger.LogInfo("Starting local file update. FileName: {FileName}, SubFolder: {SubFolder}", fileName, root ?? string.Empty);
        try
        {
            var path = BuildPath(fileName, root);
            logger.LogDebug("Built file path: {Path}", path);

            if (!File.Exists(path))
            {
                logger.LogWarn("File update failed - file not found. Path: {Path}", path);
                throw new AroFileManagementException(
                    AroFileManagementErrorCode.FILE_NOT_FOUND,
                    $"Local file '{path}' not found.");
            }

            logger.LogDebug("Opening file stream for updating. Path: {Path}", path);
            using var fs = new FileStream(path, FileMode.Create);
            content.CopyTo(fs);
            logger.LogInfo("Successfully updated local file. Path: {Path}", path);

            return Task.FromResult(path);
        }
        catch (AroFileManagementException ex)
        {
            logger.LogError(ex, "File management exception during local file update. FileName: {FileName}", fileName);
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error during local file update. FileName: {FileName}", fileName);
            throw new AroFileManagementException(
                AroFileManagementErrorCode.FILE_UPDATE_ERROR,
                "Local update failed.",
                ex);
        }
    }

    public Task<bool> DeleteFileAsync(string fileName, string? root = null)
    {
        logger.LogInfo("Starting local file deletion. FileName: {FileName}, SubFolder: {SubFolder}", fileName, root ?? string.Empty);
        try
        {
            var path = BuildPath(fileName, root);
            logger.LogDebug("Built file path: {Path}", path);

            if (!File.Exists(path))
            {
                logger.LogWarn("File deletion failed - file not found. Path: {Path}", path);
                throw new AroFileManagementException(
                    AroFileManagementErrorCode.FILE_NOT_FOUND,
                    $"Local file '{path}' not found.");
            }

            logger.LogDebug("Deleting file. Path: {Path}", path);
            File.Delete(path);
            logger.LogInfo("Successfully deleted local file. Path: {Path}", path);
            return Task.FromResult(true);
        }
        catch (AroFileManagementException ex)
        {
            logger.LogError(ex, "File management exception during local file deletion. FileName: {FileName}", fileName);
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error during local file deletion. FileName: {FileName}", fileName);
            throw new AroFileManagementException(
                AroFileManagementErrorCode.FILE_DELETE_FAILED,
                "Local delete failed.",
                ex);
        }
    }

    public string GetFileUrl(string fileName, string? root = null)
    {
        logger.LogDebug("Getting file URL. FileName: {FileName}, SubFolder: {SubFolder}", fileName, root ?? string.Empty);
        var url = BuildPath(fileName, root);
        logger.LogDebug("Generated file URL: {Url}", url);
        return url;
    }
}

