using Aro.Admin.Application.Services.DTOs.ServiceParameters.Audit;
using Aro.Admin.Domain.Entities;

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
        //string? stateBefore = null,
        //string? stateAfter = null
        string? data = null
    )
    {
        var entity = new AuditTrail
        {
            Action = action,
            ActorName = actorName,
            EntityId = entityId,
            ActorUserId = actorId,
            //After = stateAfter,
            //Before = stateBefore,
            Data = data,
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
        //string? stateBefore = null,
        //string? stateAfter = null
        string? data = null
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

        var entity = GenerateAuditTrialEntity(action, when.Value, ipAddress ?? string.Empty, actorId, actorName, entityType, entityId, /*stateBefore, stateAfter*/ data);

        return entity;
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

    private AuditTrail GenerateTrailForUserCreated(string action, Guid userId, UserCreatedLog log)
    {
        var data = serializer.Serialize(log);
        var trail = GenerateAuditTrialEntityWithCommonParams(action: action, entityType: auditEntityTypes.User, entityId: userId.ToString(), /*stateAfter: state*/ data: data);

        return trail;
    }

    private AuditTrail GenerateTrialForRolesAssigned(string action, RolesAssignedLog log)
    {
        var data = serializer.Serialize(log);
        var trial = GenerateAuditTrialEntityWithCommonParams(action: action, entityType: auditEntityTypes.User, /*stateAfter: state*/ data: data);

        return trial;
    }

    private AuditTrail GenerateTrialForRolesRevoked(string action, RolesRevokedLog log)
    {
        var data = serializer.Serialize(log);
        var trial = GenerateAuditTrialEntityWithCommonParams(action: action, entityType: auditEntityTypes.User, data: data);

        return trial;
    }

    private AuditTrail GenerateTrialForAuthenticationSuccesful(AuthenticationSuccessfulLog log)
    {
        var trail = GenerateAuditTrialEntityWithCommonParams(action: auditActions.AuthenticationSuccessful, entityType: auditEntityTypes.User, entityId: log.Email, data: serializer.Serialize(new
        {
            Email = log.Email,
            Expiry = dateFormatter.Format(log.Expiry)
        }));

        return trail;
    }

    private AuditTrail GenerateTrialForAuthenticationFailed(AuthenticationFailedLog log)
    {
        var trail = GenerateAuditTrialEntityWithCommonParams(action: auditActions.AuthenticationFailed, entityType: auditEntityTypes.User, entityId: log.Email, data: serializer.Serialize(new
        {
            log.Email
        }));

        return trail;
    }

    private AuditTrail GenerateTrailForUserSessionLoggedOut(UserSessionLoggedOutLog log) => GenerateAuditTrialEntityWithCommonParams(action: auditActions.UserSessionLoggedOut, entityType: auditEntityTypes.RefreshToken, entityId: log.RefreshTokenHash, data: serializer.Serialize(log));

    private AuditTrail GenerateTrailForUserSessionsLoggedOut(UserSessionsLoggedOutLog log) => GenerateAuditTrialEntityWithCommonParams(action: auditActions.UserSessionLoggedOut, entityType: auditEntityTypes.RefreshToken, data: serializer.Serialize(log));
}
