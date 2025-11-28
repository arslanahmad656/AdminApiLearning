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

    public async Task<Uri> CreateFileAsync(string fileName, Stream content, string? sub = null)
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
            var uri = blob.Uri;
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

    public async Task<Uri> UpdateFileAsync(string fileName, Stream content, string? sub = null)
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
            var uri = blob.Uri;
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

    public async Task<Stream> ReadFileByUriAsync(Uri uri)
    {
        logger.LogInfo("Starting blob file read by URI. Uri: {Uri}", uri);
        try
        {
            if (uri == null)
            {
                logger.LogWarn("Blob read by URI failed - URI is null.");
                throw new AroFileManagementException(
                    AroFileManagementErrorCode.FILE_READ_ERROR,
                    "URI cannot be null.");
            }

            logger.LogDebug("Validating blob URI: {Uri}", uri);

            // Validate that the URI belongs to the configured storage account and container
            var expectedHost = $"{Container.AccountName}.blob.core.windows.net";
            if (!uri.Host.Equals(expectedHost, StringComparison.OrdinalIgnoreCase))
            {
                logger.LogWarn("Blob read by URI failed - storage account mismatch. Expected: {Expected}, Actual: {Actual}", expectedHost, uri.Host);
                throw new AroFileManagementException(
                    AroFileManagementErrorCode.FILE_NOT_FOUND,
                    $"Blob '{uri}' does not exist.");
            }

            // Extract container name from URI path (format: /containername/blobpath)
            var pathSegments = uri.AbsolutePath.TrimStart('/').Split('/', 2);
            if (pathSegments.Length == 0 || !pathSegments[0].Equals(Container.Name, StringComparison.OrdinalIgnoreCase))
            {
                logger.LogWarn("Blob read by URI failed - container mismatch. Expected: {Expected}, Actual: {Actual}", Container.Name, pathSegments.Length > 0 ? pathSegments[0] : "(none)");
                throw new AroFileManagementException(
                    AroFileManagementErrorCode.FILE_NOT_FOUND,
                    $"Blob '{uri}' does not exist.");
            }

            logger.LogDebug("URI validation successful. Retrieving blob client from URI.");
            
            // Use container's blob client with same credentials
            var blobPath = pathSegments.Length > 1 ? pathSegments[1] : string.Empty;
            var blob = Container.GetBlobClient(blobPath);
            logger.LogDebug("Retrieved blob client. BlobPath: {BlobPath}", blobPath);

            if (!await blob.ExistsAsync())
            {
                logger.LogWarn("Blob read by URI failed - blob not found. Uri: {Uri}", uri);
                throw new AroFileManagementException(
                    AroFileManagementErrorCode.FILE_NOT_FOUND,
                    $"Blob '{uri}' does not exist.");
            }

            logger.LogDebug("Opening blob stream for reading by URI. Uri: {Uri}", uri);
            var stream = await blob.OpenReadAsync();
            logger.LogInfo("Successfully opened blob for reading by URI. Uri: {Uri}", uri);
            return stream;
        }
        catch (AroFileManagementException ex)
        {
            logger.LogError(ex, "File management exception during blob read by URI. Uri: {Uri}", uri);
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error during blob read by URI. Uri: {Uri}", uri);
            throw new AroFileManagementException(
                AroFileManagementErrorCode.FILE_READ_ERROR,
                "Blob read by URI failed.",
                ex);
        }
    }

    public Uri GetFileUrl(string fileName, string? sub = null)
    {
        logger.LogDebug("Getting blob URL. FileName: {FileName}, SubFolder: {SubFolder}", fileName, sub ?? string.Empty);
        var path = BuildPath(fileName, sub);
        var url = Container.GetBlobClient(path).Uri;
        logger.LogDebug("Generated blob URL: {Url}", url);
        return url;
    }
}