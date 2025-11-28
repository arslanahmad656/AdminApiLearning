using Aro.Common.Application.Repository;

namespace Aro.FileManager.Demo;

/// <summary>
/// Mock implementation of IUnitOfWork for demonstration purposes
/// </summary>
public class MockUnitOfWork : IUnitOfWork
{
    public Task SaveChanges(CancellationToken cancellationToken = default)
    {
        // Simulate successful save
        return Task.CompletedTask;
    }
}

