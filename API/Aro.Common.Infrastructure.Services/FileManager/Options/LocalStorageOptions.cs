namespace Aro.Common.Infrastructure.Services.FileManager.Options;

public record LocalStorageOptions
{
    public string Path { get; init; } = string.Empty;
}
