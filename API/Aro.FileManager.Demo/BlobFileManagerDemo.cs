using Aro.Common.Application.Services.FileManager;
using Aro.Common.Application.Services.LogManager;
using Aro.Common.Infrastructure.Services.Azure.FileManager;
using Azure.Core.Diagnostics;
using Azure.Identity;
using System.Diagnostics.Tracing;
using System.Text;

namespace Aro.FileManager.Demo;

public class BlobFileManagerDemo
{
    public static async Task Run()
    {
        Console.WriteLine("\n========================================");
        Console.WriteLine("BLOB FILE MANAGER DEMONSTRATION");
        Console.WriteLine("========================================\n");

        Console.WriteLine("NOTE: This demo requires Azure Storage credentials.");
        Console.WriteLine("      You need to provide a storage account name and container name.");
        Console.WriteLine("      The demo will use DefaultAzureCredential for authentication.\n");

        // Read configuration from user (in real scenario, this would come from config)
        Console.Write("Enter Azure Storage Account Name (or press Enter to skip): ");
        string? storageAccountName = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(storageAccountName))
        {
            Console.WriteLine("\n⚠ Blob File Manager demonstration skipped (no storage account provided).");
            Console.WriteLine("  To run this demo, you need:");
            Console.WriteLine("  - An Azure Storage Account");
            Console.WriteLine("  - A container name");
            Console.WriteLine("  - Appropriate Azure credentials configured");
            Console.WriteLine("\n========================================\n");
            return;
        }

        Console.Write("Enter Container Name: ");
        string? containerName = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(containerName))
        {
            Console.WriteLine("\n⚠ Blob File Manager demonstration skipped (no container name provided).");
            Console.WriteLine("\n========================================\n");
            return;
        }

        // Initialize logger and file manager
        ILogManager<BlobFileManager> logger = new ConsoleLogger<BlobFileManager>();
        
        string storage = storageAccountName;
        string area = containerName;
        string? root = "demo-files";
        
        try
        {
            // Enable Azure SDK logging
            using AzureEventSourceListener listener = AzureEventSourceListener.CreateConsoleLogger(EventLevel.Verbose);

            // Use DefaultAzureCredential for authentication with logging enabled
            var credentialOptions = new DefaultAzureCredentialOptions
            {
                ExcludeEnvironmentCredential = false,
                ExcludeManagedIdentityCredential = false,
                ExcludeVisualStudioCredential = false,
                ExcludeVisualStudioCodeCredential = false,
                ExcludeAzureCliCredential = false,
                ExcludeAzurePowerShellCredential = false,
                ExcludeAzureDeveloperCliCredential = false,
                ExcludeInteractiveBrowserCredential = false,
                Diagnostics =
                {
                    IsLoggingEnabled = true,
                    IsLoggingContentEnabled = true,
                    LoggedHeaderNames = { "x-ms-request-id", "x-ms-client-request-id" },
                    LoggedQueryParameters = { "api-version" },
                    IsAccountIdentifierLoggingEnabled = true,
                }
            };

            Console.WriteLine("\n🔐 Initializing Azure authentication...");
            Console.WriteLine("   Attempting authentication in the following order:");
            Console.WriteLine("   1. Environment variables");
            Console.WriteLine("   2. Managed Identity");
            Console.WriteLine("   3. Visual Studio");
            Console.WriteLine("   4. Azure CLI");
            Console.WriteLine("   5. Azure PowerShell");
            Console.WriteLine("   6. Interactive Browser");
            Console.WriteLine("\n📋 Azure SDK Diagnostic Logs:\n");
            Console.WriteLine("".PadRight(60, '-'));

            var credential = new DefaultAzureCredential(credentialOptions);
            
            IFileManager fileManager = new BlobFileManager(storage, area, root, credential, logger);

            Console.WriteLine("".PadRight(60, '-'));
            Console.WriteLine("\n✓ Successfully authenticated and connected to Azure Blob Storage\n");

            // 1. CREATE FILE
            Console.WriteLine("1. Creating a new blob...");
            string fileName = "blob-test.txt";
            string content = "Hello from Blob File Manager!\nThis file is stored in Azure Blob Storage.";
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(content)))
            {
                string blobUri = await fileManager.CreateFileAsync(fileName, stream);
                Console.WriteLine($"   ✓ Blob created at: {blobUri}\n");
            }

            // 2. READ FILE
            Console.WriteLine("2. Reading the blob...");
            using (var readStream = await fileManager.ReadFileAsync(fileName))
            using (var reader = new StreamReader(readStream))
            {
                string readContent = await reader.ReadToEndAsync();
                Console.WriteLine($"   ✓ Blob content:\n   {readContent.Replace("\n", "\n   ")}\n");
            }

            // 3. UPDATE FILE
            Console.WriteLine("3. Updating the blob...");
            string updatedContent = "Updated content in Azure Blob Storage!\nThis blob has been modified.";
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(updatedContent)))
            {
                string updatedUri = await fileManager.UpdateFileAsync(fileName, stream);
                Console.WriteLine($"   ✓ Blob updated at: {updatedUri}\n");
            }

            // 4. READ UPDATED FILE
            Console.WriteLine("4. Reading the updated blob...");
            using (var readStream = await fileManager.ReadFileAsync(fileName))
            using (var reader = new StreamReader(readStream))
            {
                string readContent = await reader.ReadToEndAsync();
                Console.WriteLine($"   ✓ Updated content:\n   {readContent.Replace("\n", "\n   ")}\n");
            }

            // 5. GET FILE URL
            Console.WriteLine("5. Getting blob URL...");
            string blobUrl = fileManager.GetFileUrl(fileName);
            Console.WriteLine($"   ✓ Blob URL: {blobUrl}\n");

            // 6. CREATE FILE IN SUBFOLDER (using virtual directory in blob storage)
            Console.WriteLine("6. Creating a blob in a virtual subdirectory...");
            string subFolder = "reports";
            string reportFileName = "monthly-report.txt";
            string reportContent = "Monthly Report\nDate: November 2025\nStatus: Complete";
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(reportContent)))
            {
                string reportUri = await fileManager.CreateFileAsync(reportFileName, stream, subFolder);
                Console.WriteLine($"   ✓ Report blob created at: {reportUri}\n");
            }

            // 7. DELETE FILE
            Console.WriteLine("7. Deleting the original blob...");
            bool deleted = await fileManager.DeleteFileAsync(fileName);
            Console.WriteLine($"   ✓ Blob deleted: {deleted}\n");

            // 8. CLEAN UP SUBFOLDER FILE
            Console.WriteLine("8. Cleaning up subfolder blob...");
            bool reportDeleted = await fileManager.DeleteFileAsync(reportFileName, subFolder);
            Console.WriteLine($"   ✓ Report blob deleted: {reportDeleted}\n");

            Console.WriteLine("✓ Blob File Manager demonstration completed successfully!");
        }
        catch (Azure.RequestFailedException ex)
        {
            Console.WriteLine($"\n✗ Azure error during demonstration: {ex.Message}");
            Console.WriteLine($"  Status: {ex.Status}");
            Console.WriteLine($"  Error Code: {ex.ErrorCode}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n✗ Error during demonstration: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"  Inner exception: {ex.InnerException.Message}");
            }
        }

        Console.WriteLine("\n========================================\n");
    }
}

