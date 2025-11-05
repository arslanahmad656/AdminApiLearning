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
        var trail = GenerateAuditTrialEntityWithCommonParams(action: auditActions.AuthenticationSuccessful, entityType: auditEntityTypes.User, entityId: log.Email, data: serializer.Serialize(log));

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

    private AuditTrail GenerateTrailForTokenRefreshed(TokenRefreshedLog log) => GenerateAuditTrialEntityWithCommonParams(action: auditActions.UserSessionLoggedOut, entityType: auditEntityTypes.RefreshToken, entityId: log.OldRefreshTokenHash, data: serializer.Serialize(log));

    private AuditTrail GenerateTrailForPasswordResetTokenGenerated(PasswordResetTokenGeneratedLog log) => GenerateAuditTrialEntityWithCommonParams(action: auditActions.PasswordResetTokenGenerated, entityType: auditEntityTypes.User, entityId: log.UserId.ToString(), data: serializer.Serialize(log));

    private AuditTrail GenerateTrailForPasswordResetCompleted(PasswordResetCompletedLog log) => GenerateAuditTrialEntityWithCommonParams(action: auditActions.PasswordResetCompleted, entityType: auditEntityTypes.User, entityId: log.UserId.ToString(), data: serializer.Serialize(log));

    private AuditTrail GenerateTrailForPasswordResetFailed(PasswordResetFailedLog log) => GenerateAuditTrialEntityWithCommonParams(action: auditActions.PasswordResetFailed, entityType: auditEntityTypes.User, entityId: log.UserId?.ToString(), data: serializer.Serialize(log));

    private AuditTrail GenerateTrailForPasswordResetLinkGenerated(PasswordResetLinkGeneratedLog log) => GenerateAuditTrialEntityWithCommonParams(action: auditActions.PasswordResetLinkGenerated, entityType: auditEntityTypes.User, entityId: log.Email, data: serializer.Serialize(log));

    private AuditTrail GenerateTrailForPasswordResetLinkGenerationFailed(PasswordResetLinkGenerationFailedLog log) => GenerateAuditTrialEntityWithCommonParams(action: auditActions.PasswordResetLinkGenerationFailed, entityType: auditEntityTypes.User, entityId: log.Email, data: serializer.Serialize(log));

    private AuditTrail GenerateTrailForPasswordChangeSuccess(ChangePasswordSuccessLog log) => GenerateAuditTrialEntityWithCommonParams(action: auditActions.PasswordChangeSuccess, entityType: auditEntityTypes.User, entityId: log.Email, data: serializer.Serialize(log));
    
    private AuditTrail GenerateTrailForPasswordChangeFailure(ChangePasswordFailedLog log) => GenerateAuditTrialEntityWithCommonParams(action: auditActions.PasswordChangeFailed, entityType: auditEntityTypes.User, entityId: log.Email, data: serializer.Serialize(log));
    #region [Entity Creation]

    private AuditTrail GenerateTrailForUserCreated(string action, Guid userId, UserCreatedLog log)
    {
        var data = serializer.Serialize(log);
        var trail = GenerateAuditTrialEntityWithCommonParams(action: action, entityType: auditEntityTypes.User, entityId: userId.ToString(), /*stateAfter: state*/ data: data);

        return trail;
    }

    private AuditTrail GenerateTrailForGroupCreated(string action, Guid groupId, GroupCreatedLog log)
    {
        var data = serializer.Serialize(log);
        var trail = GenerateAuditTrialEntityWithCommonParams(action: action, entityType: auditEntityTypes.Group, entityId: groupId.ToString(), /*stateAfter: state*/ data: data);

        return trail;
    }

    #endregion

    #region [Entity Updates]

    private AuditTrail GenerateTrailForGroupPatched(string action, Guid groupId, GroupPatchedLog log)
    {
        var data = serializer.Serialize(log);
        var trail = GenerateAuditTrialEntityWithCommonParams(action: action, entityType: auditEntityTypes.Group, entityId: groupId.ToString(), /*stateAfter: state*/ data: data);

        return trail;
    }

    #endregion

    #region [Entity Deletion]

    private AuditTrail GenerateTrailForGroupDeleted(string action, Guid groupId, GroupDeletedLog log)
    {
        var data = serializer.Serialize(log);
        var trail = GenerateAuditTrialEntityWithCommonParams(action: action, entityType: auditEntityTypes.Group, entityId: groupId.ToString(), /*stateAfter: state*/ data: data);

        return trail;
    }

    #endregion
}
