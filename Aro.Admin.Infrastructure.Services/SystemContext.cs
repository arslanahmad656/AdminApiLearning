using Aro.Admin.Application.Services;

namespace Aro.Admin.Infrastructure.Services;

public class SystemContext : ISystemContext
{
    public bool IsSystemContext { get; set; }
}
