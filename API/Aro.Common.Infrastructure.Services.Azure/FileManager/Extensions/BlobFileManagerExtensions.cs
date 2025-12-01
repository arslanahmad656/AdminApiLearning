using Aro.Common.Application.Services.FileManager;
using Aro.Common.Application.Services.LogManager;
using Aro.Common.Domain.Shared;
using Aro.Common.Domain.Shared.Exceptions;
using Aro.Common.Infrastructure.Services.Azure.FileManager.Options;
using Azure.Core;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Aro.Common.Infrastructure.Services.Azure.FileManager.Extensions;

public static class BlobFileManagerExtensions
{
    public static IServiceCollection AddBlobFileManagement(this IServiceCollection services, IConfiguration configuration)
    {
        var azureSettings = configuration
                                       .GetSection("AzureSettings")
                                       .Get<AzureSettings>() 
                                       ?? throw new AroInvalidOperationException(new ErrorCodes().UNKNOWN_ERROR, $"Could not get the app settings against AzureSettings section.");

        var blobOptions = configuration
                                       .GetSection("AzureBlobStorage")
                                       .Get<AzureBlobStorageOptions>()
                                       ?? throw new AroInvalidOperationException(new ErrorCodes().UNKNOWN_ERROR, $"Could not get the app settings against AzureBlobStorage section.");

        var options = new DefaultAzureCredentialOptions
        {
            Diagnostics =
                {
                    IsLoggingEnabled = true,
                    IsAccountIdentifierLoggingEnabled = true,
                    IsLoggingContentEnabled = true
                }
        };

        DefaultAzureCredential credentials = azureSettings.EnableCredentialLogging
            ? new(options)
            : new();

        services.AddSingleton<IFileManager>(sp => new BlobFileManager(blobOptions.StorageAccount, blobOptions.ContainerName, blobOptions.SubFolder, credentials, sp.GetRequiredService<ILogManager<BlobFileManager>>()));
        return services;
    }
}
