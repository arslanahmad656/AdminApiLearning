using Aro.Common.Domain.Shared.Exceptions;
using Azure.Core;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace Aro.Common.Infrastructure.Services.Azure.FileManager;

public partial class BlobFileManager
{
    private static BlobContainerClient InitializeContainer(string storage, string area, TokenCredential credential)
    {
        if (string.IsNullOrWhiteSpace(storage))
        {
            throw new AroFileManagementException(
                AroFileManagementErrorCode.GENERAL_FILE_ERROR,
                "Storage account name is required.");
        }

        if (string.IsNullOrWhiteSpace(area))
        {
            throw new AroFileManagementException(
                AroFileManagementErrorCode.GENERAL_FILE_ERROR,
                "Blob container name is required.");
        }

        var blobService = new BlobServiceClient(
            new Uri($"https://{storage}.blob.core.windows.net"),
            credential);

        var container = blobService.GetBlobContainerClient(area);
        container.CreateIfNotExists(PublicAccessType.None);
        return container;
    }

    private string BuildPath(string fileName, string? sub)
    {
        var parts = new List<string>();

        if (!string.IsNullOrWhiteSpace(RootPrefix))
        {
            parts.Add(RootPrefix);
        }
        if (!string.IsNullOrWhiteSpace(sub))
        {
            parts.Add(sub.Trim('/'));
        }

        parts.Add(fileName);

        return string.Join("/", parts);
    }
}
