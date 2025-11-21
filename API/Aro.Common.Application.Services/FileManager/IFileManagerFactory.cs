namespace Aro.Common.Application.Services.FileManager;

public interface IFileManagerFactory
{
    IFileManager Create(string storage, string area, string? root = null);
}
