using Aro.Common.Application.Services.FileManager;
using Aro.Common.Application.Services.LogManager;
using Aro.Common.Domain.Shared.Exceptions;
using Azure.Core;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace Aro.Common.Infrastructure.Services.Azure.FileManager;

public partial class BlobFileManager(string storage, string area, string? root, TokenCredential credential, ILogManager<BlobFileManager> logger) : IFileManager
{
    private readonly BlobContainerClient Container = InitializeContainer(storage, area, credential);
    private readonly string RootPrefix = root?.Trim('/') ?? string.Empty;

    public async Task<string> CreateFileAsync(string fileName, Stream content, string? sub = null)
    {
        logger.LogInfo("Starting blob file creation. FileName: {FileName}, SubFolder: {SubFolder}, Container: {Container}", fileName, sub ?? string.Empty, Container.Name);
        try
        {
            var path = BuildPath(fileName, sub);
            logger.LogDebug("Built blob path: {Path}", path);
            
            var blob = Container.GetBlobClient(path);
            logger.LogDebug("Retrieved blob client. BlobUri: {BlobUri}", blob.Uri);

            if (await blob.ExistsAsync())
            {
                logger.LogWarn("Blob creation failed - blob already exists. Path: {Path}", path);
                throw new AroFileManagementException(
                    AroFileManagementErrorCode.FILE_CREATION_FAILED,
                    $"Blob '{path}' already exists.");
            }

            logger.LogDebug("Uploading blob content. Path: {Path}", path);
            await blob.UploadAsync(content, overwrite: false);
            var uri = blob.Uri.ToString();
            logger.LogInfo("Successfully created blob file. Uri: {Uri}", uri);
            return uri;
        }
        catch (AroFileManagementException ex)
        {
            logger.LogError(ex, "File management exception during blob creation. FileName: {FileName}", fileName);
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error during blob creation. FileName: {FileName}, Container: {Container}", fileName, Container.Name);
            throw new AroFileManagementException(
                AroFileManagementErrorCode.FILE_CREATION_FAILED,
                "Blob creation failed.",
                ex);
        }
    }

    public async Task<Stream> ReadFileAsync(string fileName, string? sub = null)
    {
        logger.LogInfo("Starting blob file read. FileName: {FileName}, SubFolder: {SubFolder}, Container: {Container}", fileName, sub ?? string.Empty, Container.Name);
        try
        {
            var path = BuildPath(fileName, sub);
            logger.LogDebug("Built blob path: {Path}", path);
            
            var blob = Container.GetBlobClient(path);
            logger.LogDebug("Retrieved blob client. BlobUri: {BlobUri}", blob.Uri);

            if (!await blob.ExistsAsync())
            {
                logger.LogWarn("Blob read failed - blob not found. Path: {Path}", path);
                throw new AroFileManagementException(
                    AroFileManagementErrorCode.FILE_NOT_FOUND,
                    $"Blob '{path}' does not exist.");
            }

            logger.LogDebug("Opening blob stream for reading. Path: {Path}", path);
            var stream = await blob.OpenReadAsync();
            logger.LogInfo("Successfully opened blob for reading. Path: {Path}", path);
            return stream;
        }
        catch (AroFileManagementException ex)
        {
            logger.LogError(ex, "File management exception during blob read. FileName: {FileName}", fileName);
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error during blob read. FileName: {FileName}, Container: {Container}", fileName, Container.Name);
            throw new AroFileManagementException(
                AroFileManagementErrorCode.FILE_READ_ERROR,
                "Blob read failed.",
                ex);
        }
    }

    public async Task<string> UpdateFileAsync(string fileName, Stream content, string? sub = null)
    {
        logger.LogInfo("Starting blob file update. FileName: {FileName}, SubFolder: {SubFolder}, Container: {Container}", fileName, sub ?? string.Empty, Container.Name);
        try
        {
            var path = BuildPath(fileName, sub);
            logger.LogDebug("Built blob path: {Path}", path);
            
            var blob = Container.GetBlobClient(path);
            logger.LogDebug("Retrieved blob client. BlobUri: {BlobUri}", blob.Uri);

            if (!await blob.ExistsAsync())
            {
                logger.LogWarn("Blob update failed - blob not found. Path: {Path}", path);
                throw new AroFileManagementException(
                    AroFileManagementErrorCode.FILE_NOT_FOUND,
                    $"Blob '{path}' not found.");
            }

            logger.LogDebug("Uploading blob content with overwrite. Path: {Path}", path);
            await blob.UploadAsync(content, overwrite: true);
            var uri = blob.Uri.ToString();
            logger.LogInfo("Successfully updated blob file. Uri: {Uri}", uri);
            return uri;
        }
        catch (AroFileManagementException ex)
        {
            logger.LogError(ex, "File management exception during blob update. FileName: {FileName}", fileName);
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error during blob update. FileName: {FileName}, Container: {Container}", fileName, Container.Name);
            throw new AroFileManagementException(
                AroFileManagementErrorCode.FILE_UPDATE_ERROR,
                "Blob update failed.",
                ex);
        }
    }

    public async Task<bool> DeleteFileAsync(string fileName, string? sub = null)
    {
        logger.LogInfo("Starting blob file deletion. FileName: {FileName}, SubFolder: {SubFolder}, Container: {Container}", fileName, sub ?? string.Empty, Container.Name);
        try
        {
            var path = BuildPath(fileName, sub);
            logger.LogDebug("Built blob path: {Path}", path);
            
            var blob = Container.GetBlobClient(path);
            logger.LogDebug("Retrieved blob client. BlobUri: {BlobUri}", blob.Uri);

            logger.LogDebug("Attempting to delete blob. Path: {Path}", path);
            var deleted = await blob.DeleteIfExistsAsync();
            if (!deleted)
            {
                logger.LogWarn("Blob deletion failed - blob not found. Path: {Path}", path);
                throw new AroFileManagementException(
                    AroFileManagementErrorCode.FILE_NOT_FOUND,
                    $"Blob '{path}' not found.");
            }

            logger.LogInfo("Successfully deleted blob file. Path: {Path}", path);
            return true;
        }
        catch (AroFileManagementException ex)
        {
            logger.LogError(ex, "File management exception during blob deletion. FileName: {FileName}", fileName);
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error during blob deletion. FileName: {FileName}, Container: {Container}", fileName, Container.Name);
            throw new AroFileManagementException(
                AroFileManagementErrorCode.FILE_DELETE_FAILED,
                "Blob deletion failed.",
                ex);
        }
    }

    public string GetFileUrl(string fileName, string? sub = null)
    {
        logger.LogDebug("Getting blob URL. FileName: {FileName}, SubFolder: {SubFolder}", fileName, sub ?? string.Empty);
        var path = BuildPath(fileName, sub);
        var url = Container.GetBlobClient(path).Uri.ToString();
        logger.LogDebug("Generated blob URL: {Url}", url);
        return url;
    }
}