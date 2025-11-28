# ARO File Manager Demonstration

This is a console application that demonstrates the usage of file management components in the ARO system.

## Overview

The application demonstrates:

1. **LocalFileManager** - File system-based storage for local file operations
2. **InMemoryFileManager** - RAM-based storage for testing and temporary data
3. **BlobFileManager** - Azure Blob Storage integration for cloud-based file storage
4. **FileResourceService** - Coordinated storage + database operations with transaction handling

## File Structure

- `Program.cs` - Main entry point that orchestrates all demonstrations
- `ConsoleLogger.cs` - Simple logger implementation for console output
- `LocalFileManagerDemo.cs` - Demonstrates local file system operations
- `InMemoryFileManagerDemo.cs` - Demonstrates in-memory file operations
- `BlobFileManagerDemo.cs` - Demonstrates Azure Blob Storage operations
- `FileResourceServiceDemo.cs` - Demonstrates file resource service with DB integration
- `MockRepositoryManager.cs` - Mock repository for demo purposes
- `MockUnitOfWork.cs` - Mock unit of work for demo purposes
- `MockUniqueIdGenerator.cs` - Mock ID generator for demo purposes

## How to Run

### Prerequisites

- .NET 8.0 or later
- For Blob Storage demo: Azure Storage Account credentials

### Running the Application

```bash
cd Aro.FileManager.Demo
dotnet run
```

### Local File Manager Demo

This demo runs automatically and requires no configuration. It will:
- Create files in your system's temp directory
- Perform CRUD operations (Create, Read, Update, Delete)
- Clean up all created files automatically

### In-Memory File Manager Demo

This demo runs automatically and requires no configuration. It will:
- Create files in RAM (no disk I/O)
- Demonstrate all file operations in memory
- Show how the mock implementation works for testing

### Blob File Manager Demo

This demo will prompt you for:
- Azure Storage Account Name
- Container Name

**Authentication**: The demo uses `DefaultAzureCredential`, which will attempt to authenticate using:
1. Environment variables
2. Managed Identity
3. Visual Studio credentials
4. Azure CLI credentials
5. Interactive browser authentication

You can skip this demo by pressing Enter when prompted for the storage account name.

### File Resource Service Demo

This demo runs automatically with mock implementations and shows:
- Creating file resources (storage + database)
- Reading files by ID (DB verification + storage retrieval)
- Reading files by URI (DB verification + storage retrieval)
- Getting file metadata only (DB-only queries)
- Deleting files by ID with compensating transactions
- Deleting files by URI with compensating transactions
- Proper error handling when files don't exist

## What Each Demo Shows

### Common Operations (All Managers)

Each demonstration shows:

1. **Create File** - Creating a new file with content
2. **Read File** - Reading file contents back
3. **Update File** - Modifying existing file content
4. **Read Updated File** - Verifying the update
5. **Get File URL** - Getting the file location/URL
6. **Create File in Subfolder** - Organizing files in subdirectories
7. **Delete Files** - Cleaning up created files

### Unique Features

- **LocalFileManager**: Shows real file system paths and directory creation
- **InMemoryFileManager**: Demonstrates multiple file creation and in-memory storage
- **BlobFileManager**: Shows Azure blob URLs and cloud storage integration
- **FileResourceService**: Shows coordinated storage and database operations with transaction handling

## Configuration

For production use, you would typically configure these managers using:

- **LocalFileManager**: App settings for base storage path
- **InMemoryFileManager**: Used in testing/mocking scenarios
- **BlobFileManager**: Azure connection strings and managed identities

## Error Handling

Each demo includes comprehensive error handling to show:
- File management exceptions
- Azure-specific errors (for blob storage)
- Detailed error messages with context

## Logging

The application uses Serilog for structured logging:
- All file operations are logged with context
- You'll see detailed information about each step
- Errors are logged with full exception details

## Cleanup

All demos automatically clean up after themselves:
- Local files are deleted from temp directory
- In-memory files are garbage collected
- Blob storage files are deleted (if demo completes successfully)

## Notes

- This is a **throwaway project** for demonstration purposes only
- No existing code has been modified
- All implementations follow the `IFileManager` interface
- The project references the actual implementation projects

## Dependencies

The demo project references:
- `Aro.Common.Infrastructure.Services` - LocalFileManager and FileResourceService
- `Aro.Common.Infrastructure.Services.Azure` - BlobFileManager
- `Aro.Common.Infrastructure.Tests.Mocks` - InMemoryFileManager
- `Aro.Common.Application.Services` - Service interfaces
- `Aro.Common.Application.Repository` - Repository interfaces
- `Aro.Common.Domain.Entities` - Domain entities
- `Aro.Common.Infrastructure.Services.Azure` - BlobFileManager
- `Aro.Common.Infrastructure.Tests.Mocks` - InMemoryFileManager
- `Aro.Common.Application.Services` - IFileManager interface
- `Azure.Identity` - Azure authentication
- `Serilog` - Logging framework

## Troubleshooting

### "Could not load file or assembly" errors
Run `dotnet restore` to ensure all dependencies are restored.

### Azure authentication errors
Ensure you're logged in via Azure CLI (`az login`) or have appropriate credentials configured.

### Permission denied errors
The local demo uses the temp directory which should be writable. If you see permission errors, check your system's temp directory permissions.

