namespace Aro.Common.Application.Services.FileManager;

public interface IFileManager
{
    Task<Uri> CreateFileAsync(string fileName, Stream content, string? root = null);
    Task<Stream> ReadFileAsync(string fileName, string? root = null);
    Task<Uri> UpdateFileAsync(string fileName, Stream content, string? root = null);
    Task<bool> DeleteFileAsync(string fileName, string? root = null);
    Task<Stream> ReadFileByUriAsync(Uri uri);
    Uri GetFileUrl(string fileName, string? root = null);
}