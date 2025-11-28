using Aro.Common.Application.Services.UniqueIdGenerator;

namespace Aro.FileManager.Demo;

/// <summary>
/// Mock implementation of IUniqueIdGenerator for demonstration purposes
/// </summary>
public class MockUniqueIdGenerator : IUniqueIdGenerator
{
    public Guid Generate()
    {
        return Guid.NewGuid();
    }
}

