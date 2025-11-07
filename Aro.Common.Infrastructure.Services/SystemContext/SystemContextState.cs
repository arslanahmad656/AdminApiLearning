using Aro.Common.Application.Services.SystemContext;

namespace Aro.Common.Infrastructure.Services.SystemContext;

public class SystemContextState : ISystemContext
{
    public bool IsEnabled { get; private set; }

    public void Enable()
    {
        if (IsEnabled)
            throw new InvalidOperationException("System context already enabled.");
        IsEnabled = true;
    }

    public void Disable()
    {
        IsEnabled = false;
    }
}
