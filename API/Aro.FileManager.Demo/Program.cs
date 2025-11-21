using Aro.FileManager.Demo;
using Serilog;

// Configure Serilog for console output
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .CreateLogger();

try
{
    Console.WriteLine("╔════════════════════════════════════════════╗");
    Console.WriteLine("║  ARO FILE MANAGER DEMONSTRATION SUITE      ║");
    Console.WriteLine("╚════════════════════════════════════════════╝");
    Console.WriteLine();
    Console.WriteLine("This application demonstrates the usage of three file manager implementations:");
    Console.WriteLine("  1. LocalFileManager  - File system-based storage");
    Console.WriteLine("  2. InMemoryFileManager - RAM-based storage (for testing)");
    Console.WriteLine("  3. BlobFileManager   - Azure Blob Storage");
    Console.WriteLine();

    // Run all demonstrations
    await LocalFileManagerDemo.Run();
    await InMemoryFileManagerDemo.Run();
    await BlobFileManagerDemo.Run();

    Console.WriteLine("╔════════════════════════════════════════════╗");
    Console.WriteLine("║  ALL DEMONSTRATIONS COMPLETED              ║");
    Console.WriteLine("╚════════════════════════════════════════════╝");
}
catch (Exception ex)
{
    Console.WriteLine($"\n╔════════════════════════════════════════════╗");
    Console.WriteLine($"║  FATAL ERROR                               ║");
    Console.WriteLine($"╚════════════════════════════════════════════╝");
    Console.WriteLine($"\nError: {ex.Message}");
    Console.WriteLine($"\nStack Trace:\n{ex.StackTrace}");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}

return 0;
