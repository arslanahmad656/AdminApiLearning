using Aro.Common.Application.Shared;

namespace Aro.Common.Application.Services.FileResource;

public interface IFileResourceService : IService
{
    Task<CreateFileResourceResponseDto> CreateFile(CreateFileResourceDto dto, CancellationToken cancellationToken);
    Task<ReadFileResourceResponseDto> ReadFileById(Guid id, CancellationToken cancellationToken);
    Task<ReadFileResourceResponseDto> ReadFileByUri(string uri, CancellationToken cancellationToken);
    Task<bool> DeleteFileById(Guid id, CancellationToken cancellationToken);
    Task<bool> DeleteFileByUri(string uri, CancellationToken cancellationToken);
    Task<FileResourceInfoDto> GetFileInfo(Guid id, CancellationToken cancellationToken);
}

