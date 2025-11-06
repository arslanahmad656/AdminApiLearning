using Aro.Common.Application.Repository;
using Aro.Common.Application.Services.Audit;
using Aro.Common.Application.Services.LogManager;
using Aro.Common.Application.Services.RequestInterpretor;
using Aro.Common.Application.Services.Serializer;
using Aro.Common.Application.Services.UniqueIdGenerator;
using Aro.Common.Domain.Entities;
using Aro.Common.Domain.Shared;

namespace Aro.Common.Infrastructure.Services;

public partial class AuditService
(
    IUniqueIdGenerator idGenerator,
    IRequestInterpretorService requestInterpretor,
    IRepositoryManager repository,
    IUnitOfWork unitOfWork,
    ISerializer serializer,
    ILogManager<AuditService> logger
) : IAuditService
{
    private readonly IAuditTrailRepository auditTrailRepository = repository.AuditTrailRepository;

    public async Task Log(AuditEntryDto log, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Starting {MethodName}", nameof(Log));

        logger.LogDebug("Received audit entry to log @{log}", log);

        var auditTrail = new AuditTrail
        {
            Id = idGenerator.Generate(),
            Action = log.Action,
            EntityType = log.EntityType,
            EntityId = log.EntityId,
            ActorName = requestInterpretor.ExtractUsername() ?? string.Empty,
            ActorUserId = requestInterpretor.GetCurrentUserId() ?? default,
            Data = serializer.Serialize(log.Payload),
            IpAddress = requestInterpretor.RetrieveIpAddress() ?? string.Empty,
            Timestamp = DateTime.UtcNow
        };

        logger.LogDebug("Created audit trail entity.");

        await auditTrailRepository.Create(auditTrail, cancellationToken).ConfigureAwait(false);

        logger.LogDebug("Saving audit trail to the database.");
        await unitOfWork.SaveChanges(cancellationToken).ConfigureAwait(false);

        logger.LogDebug("Completed {MethodName}", nameof(Log));
    }
}
