using Aro.Admin.Application.Services.UniqueIdGenerator;
using Aro.Common.Application.Services.LogManager;

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
