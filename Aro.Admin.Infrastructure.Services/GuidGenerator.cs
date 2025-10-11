using Aro.Admin.Application.Services;

namespace Aro.Admin.Infrastructure.Services;

public class GuidGenerator : IUniqueIdGenerator
{
    public Guid Generate() => Guid.NewGuid();
}
