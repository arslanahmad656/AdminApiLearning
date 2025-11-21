using Aro.Common.Application.Services.FileManager;
using Azure.Core;
using Azure.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Aro.Common.Infrastructure.Services.Azure.FileManager.Extensions;

public static class BlobFileManagerExtensions
{
    public static IServiceCollection AddBlobFileManagement(this IServiceCollection services)
    {
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

            return new DefaultAzureCredential(options);
        });
        services.AddSingleton<IFileManagerFactory, BlobFileManagerFactory>();
        return services;
    }
}
