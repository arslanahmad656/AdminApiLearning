namespace Aro.Common.Infrastructure.Tests.Mocks;

public partial class InMemoryFileManager
{
    private static string BuildBase(string storage, string area, string? root)
    {
        storage = string.IsNullOrWhiteSpace(storage) ? "mock://root" : storage.TrimEnd('/');
        area = string.IsNullOrWhiteSpace(area) ? "" : "/" + area.Trim('/');
        root = string.IsNullOrWhiteSpace(root) ? "" : "/" + root.Trim('/');

        return $"{storage}{area}{root}";
    }

    private string BuildKey(string fileName, string? sub)
    {
        sub = string.IsNullOrWhiteSpace(sub) ? "" : "/" + sub.Trim('/');
        return $"{Base}{sub}/{fileName}";
    }
}

