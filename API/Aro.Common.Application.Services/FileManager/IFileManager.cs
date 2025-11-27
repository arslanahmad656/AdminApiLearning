namespace Aro.Common.Application.Services.FileManager;

public interface IFileManager
{
    Task<string> CreateFileAsync(string fileName, Stream content, string? root = null);
    Task<Stream> ReadFileAsync(string fileName, string? root = null);
    Task<string> UpdateFileAsync(string fileName, Stream content, string? root = null);
    Task<bool> DeleteFileAsync(string fileName, string? root = null);
    string GetFileUrl(string fileName, string? root = null);
}