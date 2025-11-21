using Aro.Common.Application.Services.FileManager;
using Aro.Common.Application.Services.LogManager;
using Aro.Common.Infrastructure.Services.FileManager;
using System.Text;

namespace Aro.FileManager.Demo;

public class LocalFileManagerDemo
{
    public static async Task Run()
    {
        Console.WriteLine("\n========================================");
        Console.WriteLine("LOCAL FILE MANAGER DEMONSTRATION");
        Console.WriteLine("========================================\n");

        // Initialize logger and file manager
        ILogManager<LocalFileManager> logger = new ConsoleLogger<LocalFileManager>();
        
        // Use temp directory for demo
        string storage = Path.Combine(Path.GetTempPath(), "AroFileManagerDemo");
        string area = "local-demo";
        string? root = "documents";
        
        IFileManager fileManager = new LocalFileManager(storage, area, root, logger);

        try
        {
            // 1. CREATE FILE
            Console.WriteLine("1. Creating a new file...");
            string fileName = "test-document.txt";
            string content = "Hello from Local File Manager!\nThis is a test document.";
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(content)))
            {
                string filePath = await fileManager.CreateFileAsync(fileName, stream);
                Console.WriteLine($"   ✓ File created at: {filePath}\n");
            }

            // 2. READ FILE
            Console.WriteLine("2. Reading the file...");
            using (var readStream = await fileManager.ReadFileAsync(fileName))
            using (var reader = new StreamReader(readStream))
            {
                string readContent = await reader.ReadToEndAsync();
                Console.WriteLine($"   ✓ File content:\n   {readContent.Replace("\n", "\n   ")}\n");
            }

            // 3. UPDATE FILE
            Console.WriteLine("3. Updating the file...");
            string updatedContent = "Updated content from Local File Manager!\nThis file has been modified.";
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(updatedContent)))
            {
                string updatedPath = await fileManager.UpdateFileAsync(fileName, stream);
                Console.WriteLine($"   ✓ File updated at: {updatedPath}\n");
            }

            // 4. READ UPDATED FILE
            Console.WriteLine("4. Reading the updated file...");
            using (var readStream = await fileManager.ReadFileAsync(fileName))
            using (var reader = new StreamReader(readStream))
            {
                string readContent = await reader.ReadToEndAsync();
                Console.WriteLine($"   ✓ Updated content:\n   {readContent.Replace("\n", "\n   ")}\n");
            }

            // 5. GET FILE URL
            Console.WriteLine("5. Getting file URL...");
            string fileUrl = fileManager.GetFileUrl(fileName);
            Console.WriteLine($"   ✓ File URL: {fileUrl}\n");

            // 6. CREATE FILE IN SUBFOLDER
            Console.WriteLine("6. Creating a file in a subfolder...");
            string subFolder = "invoices";
            string invoiceFileName = "invoice-001.txt";
            string invoiceContent = "Invoice #001\nAmount: $100.00";
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(invoiceContent)))
            {
                string invoicePath = await fileManager.CreateFileAsync(invoiceFileName, stream, subFolder);
                Console.WriteLine($"   ✓ Invoice created at: {invoicePath}\n");
            }

            // 7. DELETE FILE
            Console.WriteLine("7. Deleting the original file...");
            bool deleted = await fileManager.DeleteFileAsync(fileName);
            Console.WriteLine($"   ✓ File deleted: {deleted}\n");

            // 8. CLEAN UP SUBFOLDER FILE
            Console.WriteLine("8. Cleaning up subfolder file...");
            bool invoiceDeleted = await fileManager.DeleteFileAsync(invoiceFileName, subFolder);
            Console.WriteLine($"   ✓ Invoice deleted: {invoiceDeleted}\n");

            Console.WriteLine("✓ Local File Manager demonstration completed successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n✗ Error during demonstration: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"  Inner exception: {ex.InnerException.Message}");
            }
        }
        finally
        {
            // Clean up demo directory
            try
            {
                string demoPath = Path.Combine(storage, area);
                if (Directory.Exists(demoPath))
                {
                    Directory.Delete(demoPath, true);
                    Console.WriteLine("\n✓ Demo directory cleaned up.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n⚠ Warning: Could not clean up demo directory: {ex.Message}");
            }
        }

        Console.WriteLine("\n========================================\n");
    }
}

