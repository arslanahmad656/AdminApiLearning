using AutoFixture;
using AutoFixture.Kernel;

namespace Aro.Admin.Tests.Common;

public class TestBase : IDisposable
{
    protected readonly Fixture fixture;
    private bool disposed = false;

    public TestBase()
    {
        fixture = new Fixture();
        fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => fixture.Behaviors.Remove(b));
        fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                // Dispose managed resources here if needed
            }
            disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
