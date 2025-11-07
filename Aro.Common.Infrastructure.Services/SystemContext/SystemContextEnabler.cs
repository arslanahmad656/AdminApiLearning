using Aro.Common.Application.Services.SystemContext;

namespace Aro.Common.Infrastructure.Services.SystemContext;

public sealed class SystemContextEnabler : ISystemContextEnabler
{
    private readonly SystemContextState _state;
    private bool _disposed;

    public SystemContextEnabler(SystemContextState state)
    {
        _state = state;
        _state.Enable();
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _state.Disable();
    }
}
