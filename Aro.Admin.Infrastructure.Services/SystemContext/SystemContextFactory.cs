using Aro.Admin.Application.Services.SystemContext;
using Microsoft.Extensions.DependencyInjection;

namespace Aro.Admin.Infrastructure.Services.SystemContext;

public class SystemContextFactory(IServiceProvider provider) : ISystemContextFactory
{
    public ISystemContextEnabler Create()
        => provider.GetRequiredService<ISystemContextEnabler>();
}
