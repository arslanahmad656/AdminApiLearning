using Aro.Admin.Application.Services.SystemContext;

namespace Aro.Admin.Infrastructure.Services.SystemContext;

public sealed class SystemContext : ISystemContext, ISystemContextEnabler // sealed to prevent the overriding of Dispose method
{
    private static readonly AsyncLocal<bool> enabled = new();
    private bool disposed;

    public bool IsEnabled => enabled.Value;

    public SystemContext()
    {
        if (enabled.Value)
            throw new InvalidOperationException("System context already enabled in this request.");

        enabled.Value = true;
    }

    public void Dispose()
    {
        if (disposed)
        {
            return;
        }

        disposed = true;
        enabled.Value = false;
    }
}
