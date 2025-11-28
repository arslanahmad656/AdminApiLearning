using Aro.Common.Application.Repository;
using Aro.Common.Application.Services.FileManager;
using Aro.Common.Application.Services.FileResource;
using Aro.Common.Application.Services.LogManager;
using Aro.Common.Application.Services.UniqueIdGenerator;
using Aro.Common.Infrastructure.Services.FileResource;
using Aro.Common.Infrastructure.Tests.Mocks;
using System.Text;

namespace Aro.FileManager.Demo;

public static class FileResourceServiceDemo
{
    public static async Task Run()
    {
        Console.WriteLine("\n╔════════════════════════════════════════════╗");
        Console.WriteLine("║  FILE RESOURCE SERVICE DEMONSTRATION       ║");
        Console.WriteLine("╚════════════════════════════════════════════╝");
        Console.WriteLine("\nThis demo shows IFileResourceService with:");
        Console.WriteLine("  ✓ Create file (storage + DB)");
        Console.WriteLine("  ✓ Read file by ID");
        Console.WriteLine("  ✓ Read file by URI");
        Console.WriteLine("  ✓ Get file info (metadata only)");
        Console.WriteLine("  ✓ Delete file by ID (with transaction handling)");
        Console.WriteLine("  ✓ Delete file by URI (with transaction handling)");
        Console.WriteLine();

        var logger = new ConsoleLogger<FileResourceService>();
        var fileManagerLogger = new ConsoleLogger<InMemoryFileManager>();

        // Create mock implementations
        var fileManager = new InMemoryFileManager("demo-storage", "file-resources", null, fileManagerLogger);
        var mockRepository = new MockRepositoryManager();
        var mockUnitOfWork = new MockUnitOfWork();
        var mockIdGenerator = new MockUniqueIdGenerator();
        
        var service = new FileResourceService(
            fileManager,
            mockRepository,
            mockUnitOfWork,
            mockIdGenerator,
            logger
        );

        try
        {
            // 1. Create a file
            Console.WriteLine("─────────────────────────────────────────────");
            Console.WriteLine("1️⃣  Creating File Resource...");
            Console.WriteLine("─────────────────────────────────────────────");
            
            var createDto = new CreateFileResourceDto(
                FileName: "demo-document.txt",
                Content: new MemoryStream(Encoding.UTF8.GetBytes("Hello from FileResourceService! This is demo content.")),
                Description: "Demo document for testing",
                Metadata: "{\"category\": \"demo\", \"version\": 1}",
                SubDirectory: null
            );

            var createResponse = await service.CreateFile(createDto, CancellationToken.None);
            Console.WriteLine($"✓ Created file with ID: {createResponse.Id}");
            Console.WriteLine($"  Name: {createResponse.Name}");
            Console.WriteLine($"  URI: {createResponse.Uri}");
            Console.WriteLine();

            // 2. Get file info (metadata only - no storage access)
            Console.WriteLine("─────────────────────────────────────────────");
            Console.WriteLine("2️⃣  Getting File Info (DB only)...");
            Console.WriteLine("─────────────────────────────────────────────");
            
            var fileInfo = await service.GetFileInfo(createResponse.Id, CancellationToken.None);
            Console.WriteLine($"✓ Retrieved file info:");
            Console.WriteLine($"  ID: {fileInfo.Id}");
            Console.WriteLine($"  Name: {fileInfo.Name}");
            Console.WriteLine($"  Description: {fileInfo.Description}");
            Console.WriteLine($"  Metadata: {fileInfo.Metadata}");
            Console.WriteLine($"  Created: {fileInfo.CreatedOn}");
            Console.WriteLine($"  Updated: {fileInfo.UpdatedOn?.ToString() ?? "Never"}");
            Console.WriteLine();

            // 3. Read file by ID
            Console.WriteLine("─────────────────────────────────────────────");
            Console.WriteLine("3️⃣  Reading File by ID (DB + Storage)...");
            Console.WriteLine("─────────────────────────────────────────────");
            
            var readByIdResponse = await service.ReadFileById(createResponse.Id, CancellationToken.None);
            using (var reader = new StreamReader(readByIdResponse.Content))
            {
                var content = await reader.ReadToEndAsync();
                Console.WriteLine($"✓ Read file by ID:");
                Console.WriteLine($"  Content: {content}");
                Console.WriteLine($"  Size: {content.Length} characters");
            }
            Console.WriteLine();

            // 4. Read file by URI
            Console.WriteLine("─────────────────────────────────────────────");
            Console.WriteLine("4️⃣  Reading File by URI (DB + Storage)...");
            Console.WriteLine("─────────────────────────────────────────────");
            
            var readByUriResponse = await service.ReadFileByUri(createResponse.Uri.ToString(), CancellationToken.None);
            using (var reader = new StreamReader(readByUriResponse.Content))
            {
                var content = await reader.ReadToEndAsync();
                Console.WriteLine($"✓ Read file by URI:");
                Console.WriteLine($"  URI: {readByUriResponse.Uri}");
                Console.WriteLine($"  Content: {content}");
            }
            Console.WriteLine();

            // 5. Create another file for delete by URI demo
            Console.WriteLine("─────────────────────────────────────────────");
            Console.WriteLine("5️⃣  Creating Second File for Delete Demo...");
            Console.WriteLine("─────────────────────────────────────────────");
            
            var createDto2 = new CreateFileResourceDto(
                FileName: "demo-document-2.txt",
                Content: new MemoryStream(Encoding.UTF8.GetBytes("This file will be deleted by URI.")),
                Description: "Second demo document",
                Metadata: "{}",
                SubDirectory: null
            );

            var createResponse2 = await service.CreateFile(createDto2, CancellationToken.None);
            Console.WriteLine($"✓ Created second file with ID: {createResponse2.Id}");
            Console.WriteLine($"  URI: {createResponse2.Uri}");
            Console.WriteLine();

            // 6. Delete file by URI (with transaction handling)
            Console.WriteLine("─────────────────────────────────────────────");
            Console.WriteLine("6️⃣  Deleting File by URI...");
            Console.WriteLine("─────────────────────────────────────────────");
            
            var deletedByUri = await service.DeleteFileByUri(createResponse2.Uri.ToString(), CancellationToken.None);
            Console.WriteLine($"✓ Deleted file by URI: {deletedByUri}");
            Console.WriteLine($"  (Includes compensating transaction handling)");
            Console.WriteLine();

            // 7. Verify deletion - should throw exception
            Console.WriteLine("─────────────────────────────────────────────");
            Console.WriteLine("7️⃣  Verifying Deletion (Should Fail)...");
            Console.WriteLine("─────────────────────────────────────────────");
            
            try
            {
                await service.GetFileInfo(createResponse2.Id, CancellationToken.None);
                Console.WriteLine("✗ File still exists - deletion failed!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✓ Verified deletion - file not found (as expected)");
                Console.WriteLine($"  Error: {ex.Message}");
            }
            Console.WriteLine();

            // 8. Delete file by ID (with transaction handling)
            Console.WriteLine("─────────────────────────────────────────────");
            Console.WriteLine("8️⃣  Deleting First File by ID...");
            Console.WriteLine("─────────────────────────────────────────────");
            
            var deletedById = await service.DeleteFileById(createResponse.Id, CancellationToken.None);
            Console.WriteLine($"✓ Deleted file by ID: {deletedById}");
            Console.WriteLine($"  (Storage deleted, DB updated with transaction)");
            Console.WriteLine();

            // Summary
            Console.WriteLine("─────────────────────────────────────────────");
            Console.WriteLine("✓ FILE RESOURCE SERVICE DEMO COMPLETED");
            Console.WriteLine("─────────────────────────────────────────────");
            Console.WriteLine("\nKey Features Demonstrated:");
            Console.WriteLine("  ✓ Coordinated storage + database operations");
            Console.WriteLine("  ✓ Read by ID and URI with DB verification");
            Console.WriteLine("  ✓ Metadata-only queries (no storage access)");
            Console.WriteLine("  ✓ Delete with compensating transactions");
            Console.WriteLine("  ✓ Comprehensive error handling and logging");
            Console.WriteLine();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n✗ Demo encountered an error:");
            Console.WriteLine($"  {ex.Message}");
            Console.WriteLine($"\nStack Trace:");
            Console.WriteLine($"  {ex.StackTrace}");
        }
    }
}

