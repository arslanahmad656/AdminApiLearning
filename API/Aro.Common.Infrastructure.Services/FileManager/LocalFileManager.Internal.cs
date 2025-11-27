using Aro.Common.Domain.Shared.Exceptions;

namespace Aro.Common.Infrastructure.Services.FileManager;

public partial class LocalFileManager
{
    private static string BuildRoot(string storage, string area, string? root)
    {
        if (string.IsNullOrWhiteSpace(storage))
        {
            throw new AroFileManagementException(
                AroFileManagementErrorCode.GENERAL_FILE_ERROR,
                "Base local storage path is required.");
        }

        var path = storage;

        if (!string.IsNullOrWhiteSpace(area))
        {
            path = Path.Combine(path, area);
        }

        if (!string.IsNullOrWhiteSpace(root))
        {
            path = Path.Combine(path, root);
        }

        Directory.CreateDirectory(path);
        return path;
    }

    private string BuildPath(string fileName, string? subFolder)
    {
        var path = Root;

        if (!string.IsNullOrWhiteSpace(subFolder))
        {
            path = Path.Combine(path, subFolder);
            Directory.CreateDirectory(path);
        }

        return Path.Combine(path, fileName);
    }
}
