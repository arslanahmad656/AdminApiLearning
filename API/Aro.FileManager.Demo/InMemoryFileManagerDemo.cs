using Aro.Common.Application.Services.FileManager;
using Aro.Common.Application.Services.LogManager;
using Aro.Common.Infrastructure.Tests.Mocks;
using System.Text;

namespace Aro.FileManager.Demo;

public class InMemoryFileManagerDemo
{
    public static async Task Run()
    {
        Console.WriteLine("\n========================================");
        Console.WriteLine("IN-MEMORY FILE MANAGER DEMONSTRATION");
        Console.WriteLine("========================================\n");

        // Initialize logger and file manager
        ILogManager<InMemoryFileManager> logger = new ConsoleLogger<InMemoryFileManager>();
        
        string storage = "mock://storage";
        string area = "memory-demo";
        string? root = "test-files";
        
        IFileManager fileManager = new InMemoryFileManager(storage, area, root, logger);

        try
        {
            // 1. CREATE FILE
            Console.WriteLine("1. Creating a new file in memory...");
            string fileName = "memory-test.txt";
            string content = "Hello from In-Memory File Manager!\nThis file exists only in RAM.";
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(content)))
            {
                var fileKey = await fileManager.CreateFileAsync(fileName, stream);
                Console.WriteLine($"   ✓ File created with key: {fileKey}\n");
            }

            // 2. READ FILE
            Console.WriteLine("2. Reading the file from memory...");
            using (var readStream = await fileManager.ReadFileAsync(fileName))
            using (var reader = new StreamReader(readStream))
            {
                string readContent = await reader.ReadToEndAsync();
                Console.WriteLine($"   ✓ File content:\n   {readContent.Replace("\n", "\n   ")}\n");
            }

            // 3. UPDATE FILE
            Console.WriteLine("3. Updating the file in memory...");
            string updatedContent = "Updated content in memory!\nThis demonstrates the update capability.";
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(updatedContent)))
            {
                var updatedKey = await fileManager.UpdateFileAsync(fileName, stream);
                Console.WriteLine($"   ✓ File updated with key: {updatedKey}\n");
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
            Console.WriteLine("5. Getting file URL (mock URL for in-memory)...");
            var fileUrl = fileManager.GetFileUrl(fileName);
            Console.WriteLine($"   ✓ File URL: {fileUrl}\n");

            // 6. CREATE FILE IN SUBFOLDER
            Console.WriteLine("6. Creating a file in a virtual subfolder...");
            string subFolder = "cache";
            string cacheFileName = "cached-data.json";
            string cacheContent = "{ \"id\": 1, \"name\": \"Test Data\", \"cached\": true }";
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(cacheContent)))
            {
                var cacheKey = await fileManager.CreateFileAsync(cacheFileName, stream, subFolder);
                Console.WriteLine($"   ✓ Cache file created with key: {cacheKey}\n");
            }

            // 7. CREATE MULTIPLE FILES
            Console.WriteLine("7. Creating multiple files to demonstrate storage...");
            for (int i = 1; i <= 3; i++)
            {
                string multiFileName = $"file-{i}.txt";
                string multiContent = $"Content of file number {i}";
                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(multiContent)))
                {
                    await fileManager.CreateFileAsync(multiFileName, stream);
                    Console.WriteLine($"   ✓ Created: {multiFileName}");
                }
            }
            Console.WriteLine();

            // 8. DELETE FILES
            Console.WriteLine("8. Deleting files from memory...");
            bool deleted = await fileManager.DeleteFileAsync(fileName);
            Console.WriteLine($"   ✓ Original file deleted: {deleted}");
            
            bool cacheDeleted = await fileManager.DeleteFileAsync(cacheFileName, subFolder);
            Console.WriteLine($"   ✓ Cache file deleted: {cacheDeleted}");

            // Clean up the multiple files
            for (int i = 1; i <= 3; i++)
            {
                await fileManager.DeleteFileAsync($"file-{i}.txt");
            }
            Console.WriteLine($"   ✓ Multiple files cleaned up\n");

            Console.WriteLine("✓ In-Memory File Manager demonstration completed successfully!");
            Console.WriteLine("  Note: All files existed only in RAM and are now gone.");
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

