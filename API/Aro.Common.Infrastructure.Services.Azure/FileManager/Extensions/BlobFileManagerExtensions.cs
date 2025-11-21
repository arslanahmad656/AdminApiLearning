using Aro.Common.Application.Services.FileManager;
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

        services.AddSingleton<TokenCredential>(_ =>
        {
            var options = new DefaultAzureCredentialOptions
            {
                Diagnostics =
                {
                    IsLoggingEnabled = true,
                    IsAccountIdentifierLoggingEnabled = true,
                    IsLoggingContentEnabled = true
                }
            };

            DefaultAzureCredential creds = azureSettings.EnableCredentialLogging
                ? new (options)
                : new ();

            return creds;
        });
        services.AddSingleton<IFileManagerFactory, BlobFileManagerFactory>();
        return services;
    }
}
