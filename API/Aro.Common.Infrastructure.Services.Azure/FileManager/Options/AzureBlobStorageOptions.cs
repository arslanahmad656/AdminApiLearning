namespace Aro.Common.Infrastructure.Services.Azure.FileManager.Options;

public record AzureBlobStorageOptions
{
    public string StorageAccount { get; init; } = string.Empty;
    public string ContainerName { get; init; } = string.Empty;
    public string SubFolder { get; init; } = string.Empty;
}
