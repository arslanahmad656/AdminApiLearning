using Aro.Admin.Application.Services.LogManager;
using Aro.Admin.Application.Services.UniqueIdGenerator;

namespace Aro.Admin.Infrastructure.Services;

public class GuidGenerator(ILogManager<GuidGenerator> logger) : IUniqueIdGenerator
{
    public Guid Generate()
    {
        logger.LogDebug("Starting {MethodName}", nameof(Generate));
        
        var guid = Guid.NewGuid();
        logger.LogDebug("Generated GUID: {Guid}", guid);
        
        logger.LogDebug("Completed {MethodName}", nameof(Generate));
        return guid;
    }
}
