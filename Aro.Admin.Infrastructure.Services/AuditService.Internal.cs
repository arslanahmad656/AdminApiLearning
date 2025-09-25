using Aro.Admin.Application.Services.DTOs.ServiceParameters.Audit;
using Aro.Admin.Domain.Entities;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Aro.Admin.Infrastructure.Services;

public partial class AuditService
{
    private AuditTrail GenerateAuditTrialEntity
    (
        string action,
        DateTime when,
        string ipAddress,
        Guid? actorId = null,
        string? actorName = null,
        string? entityType = null,
        string? entityId = null,
        string? stateBefore = null,
        string? stateAfter = null
    )
    {
        var entity = new AuditTrail
        {
            Action = action,
            ActorName = actorName,
            EntityId = entityId,
            ActorUserId = actorId,
            After = stateAfter,
            Before = stateBefore,
            EntityType = entityType,
            IpAddress = ipAddress,
            Timestamp = when,
            Id = idGenerator.Generate(),
        };

        return entity;
    }

    private AuditTrail GenerateAuditTrialEntityWithCommonParams
    (
        string action,
        DateTime? when = null,
        string? ipAddress = null,
        Guid? actorId = null,
        string? actorName = null,
        string? entityType = null,
        string? entityId = null,
        string? stateBefore = null,
        string? stateAfter = null
    )
    {
        var commonParams = GetCommonParameters();

        when ??= commonParams.When;
        actorName ??= commonParams.Username;
        ipAddress ??= commonParams.IPAddress;

        if (actorId is null)
        {
            if (Guid.TryParse(commonParams.UserId, out var userId))
            {
                actorId = userId;
            }
        }

        var entity = GenerateAuditTrialEntity(action, when.Value, ipAddress ?? string.Empty, actorId, actorName, entityType, entityId, stateBefore, stateAfter);

        return entity;
    }

    private AuditTrail GenerateTrailForUserCreated(string action, Guid userId, UserCreatedLog log)
    {
        var state = serializer.Serialize(log);
        var trail = GenerateAuditTrialEntityWithCommonParams(action: action, entityType: auditEntityTypes.User, entityId: userId.ToString(), stateAfter: state);

        return trail;
    }

    (string? UserId, string? Username, string? IPAddress, DateTime When) GetCommonParameters()
    {
        var userId = requestInterpretor.ExtractUsername();
        var username = userId;
        var ipAddress = requestInterpretor.RetrieveIpAddress();
        var when = DateTime.Now;

        return (userId, username, ipAddress, when);
    }

    private async Task CreateTrial(AuditTrail trial, CancellationToken cancellationToken)
    {
        await repository.AuditTrailRepository.Create(trial, cancellationToken).ConfigureAwait(false);

        await repository.SaveChanges(cancellationToken).ConfigureAwait(false);
    }
}
