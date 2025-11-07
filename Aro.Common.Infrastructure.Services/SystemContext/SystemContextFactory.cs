using Aro.Common.Application.Services.SystemContext;
using Microsoft.Extensions.DependencyInjection;

namespace Aro.Common.Infrastructure.Services.SystemContext;

public class SystemContextFactory(IServiceProvider provider) : ISystemContextFactory
{
    public ISystemContextEnabler Create()
        => provider.GetRequiredService<ISystemContextEnabler>();
}
