using Aro.Admin.Application.Services;

namespace Aro.Admin.Infrastructure.Services;

public class GuidEntityIdGenerator : IEntityIdGenerator
{
    public Guid Generate() => Guid.NewGuid();
}
