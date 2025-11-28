using Aro.Common.Application.Repository;
using Aro.Common.Application.Services.FileManager;
using Aro.Common.Application.Services.FileResource;
using Aro.Common.Application.Services.LogManager;
using Aro.Common.Application.Services.UniqueIdGenerator;
using Aro.Common.Domain.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Aro.Common.Infrastructure.Services.FileResource;

public class FileResourceService(
    IFileManager fileManager,
    IRepositoryManager repositoryManager,
    IUnitOfWork unitOfWork,
    IUniqueIdGenerator idGenerator,
    ILogManager<FileResourceService> logger
) : IFileResourceService
{
    public async Task<CreateFileResourceResponseDto> CreateFile(CreateFileResourceDto dto, CancellationToken cancellationToken)
    {
        logger.LogInfo("Starting file resource creation. FileName: {FileName}, SubDirectory: {SubDirectory}",
            dto.FileName,
            dto.SubDirectory ?? string.Empty);

        try
        {
            logger.LogDebug("Creating file using IFileManager. FileName: {FileName}", dto.FileName);

            var uri = await fileManager.CreateFileAsync(
                dto.FileName,
                dto.Content,
                dto.SubDirectory
            ).ConfigureAwait(false);

            logger.LogDebug("File created successfully. Uri: {Uri}", uri);

            var fileResourceId = idGenerator.Generate();
            var fileResource = new Domain.Entities.FileResource
            {
                Id = fileResourceId,
                Name = dto.FileName,
                Uri = uri,
                Description = dto.Description ?? string.Empty,
                Metadata = dto.Metadata ?? string.Empty,
                CreatedOn = DateTime.UtcNow,
                UpdatedOn = null
            };

            logger.LogDebug("Adding file resource to repository. Id: {Id}", fileResourceId);

            await repositoryManager.FileResourceRepository.Create(fileResource, cancellationToken).ConfigureAwait(false);

            logger.LogDebug("Saving file resource to database.");
            await unitOfWork.SaveChanges(cancellationToken).ConfigureAwait(false);

            logger.LogInfo("Successfully created file resource. Id: {Id}, Uri: {Uri}", fileResourceId, uri);

            return new CreateFileResourceResponseDto(
                fileResourceId,
                dto.FileName,
                uri
            );
        }
        catch (AroFileManagementException ex)
        {
            logger.LogError(ex, "File management error during file resource creation. FileName: {FileName}", dto.FileName);
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error during file resource creation. FileName: {FileName}", dto.FileName);
            throw new AroFileManagementException(
                AroFileManagementErrorCode.FILE_CREATION_FAILED,
                "Failed to create file resource.",
                ex
            );
        }
    }

    public async Task<ReadFileResourceResponseDto> ReadFileById(Guid id, CancellationToken cancellationToken)
    {
        logger.LogInfo("Starting file resource read by ID. Id: {Id}", id);

        try
        {
            logger.LogDebug("Querying database for file resource. Id: {Id}", id);
            var fileResource = await repositoryManager.FileResourceRepository
                .GetById(id)
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);

            if (fileResource == null)
            {
                logger.LogWarn("File resource not found in database. Id: {Id}", id);
                throw new AroFileManagementException(
                    AroFileManagementErrorCode.FILE_NOT_FOUND,
                    $"File resource with ID '{id}' not found.");
            }

            logger.LogDebug("File resource found. Id: {Id}, Uri: {Uri}", fileResource.Id, fileResource.Uri);

            logger.LogDebug("Reading file content from storage. Uri: {Uri}", fileResource.Uri);
            var uri = new Uri(fileResource.Uri);
            var content = await fileManager.ReadFileByUriAsync(uri).ConfigureAwait(false);

            logger.LogInfo("Successfully read file resource by ID. Id: {Id}", id);

            return new ReadFileResourceResponseDto(
                fileResource.Id,
                fileResource.Name,
                fileResource.Uri,
                fileResource.Description,
                fileResource.Metadata,
                fileResource.CreatedOn,
                fileResource.UpdatedOn,
                content
            );
        }
        catch (AroFileManagementException ex)
        {
            logger.LogError(ex, "File management error during file resource read by ID. Id: {Id}", id);
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error during file resource read by ID. Id: {Id}", id);
            throw new AroFileManagementException(
                AroFileManagementErrorCode.FILE_READ_ERROR,
                "Failed to read file resource.",
                ex
            );
        }
    }

    public async Task<ReadFileResourceResponseDto> ReadFileByUri(string uri, CancellationToken cancellationToken)
    {
        logger.LogInfo("Starting file resource read by URI. Uri: {Uri}", uri);

        try
        {
            logger.LogDebug("Verifying URI exists in database. Uri: {Uri}", uri);
            var fileResource = await repositoryManager.FileResourceRepository
                .GetByUri(uri)
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);

            if (fileResource == null)
            {
                logger.LogWarn("File resource with URI not found in database. Uri: {Uri}", uri);
                throw new AroFileManagementException(
                    AroFileManagementErrorCode.FILE_NOT_FOUND,
                    $"File resource with URI '{uri}' not found.");
            }

            logger.LogDebug("File resource found. Id: {Id}, Uri: {Uri}", fileResource.Id, fileResource.Uri);

            logger.LogDebug("Reading file content from storage. Uri: {Uri}", uri);
            var uriObj = new Uri(uri);
            var content = await fileManager.ReadFileByUriAsync(uriObj).ConfigureAwait(false);

            logger.LogInfo("Successfully read file resource by URI. Uri: {Uri}", uri);

            return new ReadFileResourceResponseDto(
                fileResource.Id,
                fileResource.Name,
                fileResource.Uri,
                fileResource.Description,
                fileResource.Metadata,
                fileResource.CreatedOn,
                fileResource.UpdatedOn,
                content
            );
        }
        catch (AroFileManagementException ex)
        {
            logger.LogError(ex, "File management error during file resource read by URI. Uri: {Uri}", uri);
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error during file resource read by URI. Uri: {Uri}", uri);
            throw new AroFileManagementException(
                AroFileManagementErrorCode.FILE_READ_ERROR,
                "Failed to read file resource.",
                ex
            );
        }
    }

    public async Task<bool> DeleteFileById(Guid id, CancellationToken cancellationToken)
    {
        logger.LogInfo("Starting file resource deletion by ID. Id: {Id}", id);

        try
        {
            logger.LogDebug("Querying database for file resource. Id: {Id}", id);
            var fileResource = await repositoryManager.FileResourceRepository
                .GetById(id)
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);

            if (fileResource == null)
            {
                logger.LogWarn("File resource not found in database. Id: {Id}", id);
                throw new AroFileManagementException(
                    AroFileManagementErrorCode.FILE_NOT_FOUND,
                    $"File resource with ID '{id}' not found.");
            }

            logger.LogDebug("File resource found. Id: {Id}, Uri: {Uri}", fileResource.Id, fileResource.Uri);

            // First, read the file content for potential rollback
            logger.LogDebug("Reading file content for potential rollback. Uri: {Uri}", fileResource.Uri);
            Stream? fileContentBackup = null;
            try
            {
                var uri = new Uri(fileResource.Uri);
                fileContentBackup = await fileManager.ReadFileByUriAsync(uri).ConfigureAwait(false);
                
                // Copy to memory stream so we can reuse it
                var memoryStream = new MemoryStream();
                await fileContentBackup.CopyToAsync(memoryStream, cancellationToken).ConfigureAwait(false);
                memoryStream.Position = 0;
                fileContentBackup.Dispose();
                fileContentBackup = memoryStream;
                
                logger.LogDebug("File content backed up for potential rollback. Size: {Size} bytes", memoryStream.Length);
            }
            catch (Exception ex)
            {
                logger.LogWarn(ex, "Failed to backup file content. Proceeding with deletion. Uri: {Uri}", fileResource.Uri);
                fileContentBackup?.Dispose();
                fileContentBackup = null;
            }

            // Delete file from storage
            // Note: Passing null for subdirectory assumes file is at root level.
            // If subdirectory support is needed, consider storing it in FileResource entity or Metadata field.
            logger.LogDebug("Deleting file from storage. Uri: {Uri}", fileResource.Uri);
            var uri2 = new Uri(fileResource.Uri);
            await fileManager.DeleteFileAsync(fileResource.Name, null).ConfigureAwait(false);

            logger.LogDebug("File deleted from storage. Deleting record from database. Id: {Id}", id);

            // Delete record from DB
            repositoryManager.FileResourceRepository.Delete(fileResource);

            // Try to save changes
            try
            {
                await unitOfWork.SaveChanges(cancellationToken).ConfigureAwait(false);
                logger.LogInfo("Successfully deleted file resource by ID. Id: {Id}", id);
                fileContentBackup?.Dispose();
                return true;
            }
            catch (Exception dbEx)
            {
                logger.LogError(dbEx, "Database save failed after storage deletion. Attempting to restore file. Id: {Id}", id);

                // Try to restore the file
                if (fileContentBackup != null)
                {
                    try
                    {
                        fileContentBackup.Position = 0;
                        await fileManager.CreateFileAsync(fileResource.Name, fileContentBackup, null).ConfigureAwait(false);
                        logger.LogInfo("Successfully restored file after database failure. Id: {Id}", id);
                    }
                    catch (Exception restoreEx)
                    {
                        logger.LogError(restoreEx, "Failed to restore file after database failure. Orphaned file state possible. Id: {Id}, Uri: {Uri}", id, fileResource.Uri);
                    }
                    finally
                    {
                        fileContentBackup.Dispose();
                    }
                }

                throw new AroFileManagementException(
                    AroFileManagementErrorCode.FILE_DELETE_FAILED,
                    "Failed to delete file resource from database.",
                    dbEx
                );
            }
        }
        catch (AroFileManagementException ex)
        {
            logger.LogError(ex, "File management error during file resource deletion by ID. Id: {Id}", id);
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error during file resource deletion by ID. Id: {Id}", id);
            throw new AroFileManagementException(
                AroFileManagementErrorCode.FILE_DELETE_FAILED,
                "Failed to delete file resource.",
                ex
            );
        }
    }

    public async Task<bool> DeleteFileByUri(string uri, CancellationToken cancellationToken)
    {
        logger.LogInfo("Starting file resource deletion by URI. Uri: {Uri}", uri);

        try
        {
            logger.LogDebug("Verifying URI exists in database. Uri: {Uri}", uri);
            var fileResource = await repositoryManager.FileResourceRepository
                .GetByUri(uri)
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);

            if (fileResource == null)
            {
                logger.LogWarn("File resource with URI not found in database. Uri: {Uri}", uri);
                throw new AroFileManagementException(
                    AroFileManagementErrorCode.FILE_NOT_FOUND,
                    $"File resource with URI '{uri}' not found.");
            }

            logger.LogDebug("File resource found. Id: {Id}, Uri: {Uri}", fileResource.Id, fileResource.Uri);

            // First, read the file content for potential rollback
            logger.LogDebug("Reading file content for potential rollback. Uri: {Uri}", uri);
            Stream? fileContentBackup = null;
            try
            {
                var uriObj = new Uri(uri);
                fileContentBackup = await fileManager.ReadFileByUriAsync(uriObj).ConfigureAwait(false);
                
                // Copy to memory stream so we can reuse it
                var memoryStream = new MemoryStream();
                await fileContentBackup.CopyToAsync(memoryStream, cancellationToken).ConfigureAwait(false);
                memoryStream.Position = 0;
                fileContentBackup.Dispose();
                fileContentBackup = memoryStream;
                
                logger.LogDebug("File content backed up for potential rollback. Size: {Size} bytes", memoryStream.Length);
            }
            catch (Exception ex)
            {
                logger.LogWarn(ex, "Failed to backup file content. Proceeding with deletion. Uri: {Uri}", uri);
                fileContentBackup?.Dispose();
                fileContentBackup = null;
            }

            // Delete file from storage
            // Note: Passing null for subdirectory assumes file is at root level.
            // If subdirectory support is needed, consider storing it in FileResource entity or Metadata field.
            logger.LogDebug("Deleting file from storage. Uri: {Uri}", uri);
            var uriObj2 = new Uri(uri);
            await fileManager.DeleteFileAsync(fileResource.Name, null).ConfigureAwait(false);

            logger.LogDebug("File deleted from storage. Deleting record from database. Id: {Id}", fileResource.Id);

            // Delete record from DB
            repositoryManager.FileResourceRepository.Delete(fileResource);

            // Try to save changes
            try
            {
                await unitOfWork.SaveChanges(cancellationToken).ConfigureAwait(false);
                logger.LogInfo("Successfully deleted file resource by URI. Uri: {Uri}", uri);
                fileContentBackup?.Dispose();
                return true;
            }
            catch (Exception dbEx)
            {
                logger.LogError(dbEx, "Database save failed after storage deletion. Attempting to restore file. Uri: {Uri}", uri);

                // Try to restore the file
                if (fileContentBackup != null)
                {
                    try
                    {
                        fileContentBackup.Position = 0;
                        await fileManager.CreateFileAsync(fileResource.Name, fileContentBackup, null).ConfigureAwait(false);
                        logger.LogInfo("Successfully restored file after database failure. Uri: {Uri}", uri);
                    }
                    catch (Exception restoreEx)
                    {
                        logger.LogError(restoreEx, "Failed to restore file after database failure. Orphaned file state possible. Uri: {Uri}", uri);
                    }
                    finally
                    {
                        fileContentBackup.Dispose();
                    }
                }

                throw new AroFileManagementException(
                    AroFileManagementErrorCode.FILE_DELETE_FAILED,
                    "Failed to delete file resource from database.",
                    dbEx
                );
            }
        }
        catch (AroFileManagementException ex)
        {
            logger.LogError(ex, "File management error during file resource deletion by URI. Uri: {Uri}", uri);
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error during file resource deletion by URI. Uri: {Uri}", uri);
            throw new AroFileManagementException(
                AroFileManagementErrorCode.FILE_DELETE_FAILED,
                "Failed to delete file resource.",
                ex
            );
        }
    }

    public async Task<FileResourceInfoDto> GetFileInfo(Guid id, CancellationToken cancellationToken)
    {
        logger.LogInfo("Starting file resource info retrieval by ID. Id: {Id}", id);

        try
        {
            logger.LogDebug("Querying database for file resource. Id: {Id}", id);
            var fileResource = await repositoryManager.FileResourceRepository
                .GetById(id)
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);

            if (fileResource == null)
            {
                logger.LogWarn("File resource not found in database. Id: {Id}", id);
                throw new AroFileManagementException(
                    AroFileManagementErrorCode.FILE_NOT_FOUND,
                    $"File resource with ID '{id}' not found.");
            }

            logger.LogDebug("File resource found. Mapping to DTO. Id: {Id}", id);

            var fileResourceInfo = new FileResourceInfoDto(
                fileResource.Id,
                fileResource.Name,
                fileResource.Uri,
                fileResource.Description,
                fileResource.Metadata,
                fileResource.CreatedOn,
                fileResource.UpdatedOn
            );

            logger.LogInfo("Successfully retrieved file resource info by ID. Id: {Id}", id);

            return fileResourceInfo;
        }
        catch (AroFileManagementException ex)
        {
            logger.LogError(ex, "File management error during file resource info retrieval. Id: {Id}", id);
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error during file resource info retrieval. Id: {Id}", id);
            throw new AroFileManagementException(
                AroFileManagementErrorCode.FILE_READ_ERROR,
                "Failed to retrieve file resource info.",
                ex
            );
        }
    }
}

