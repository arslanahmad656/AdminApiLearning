using Aro.Admin.Application.Services.Audit;
using Aro.Admin.Application.Services.DTOs.ServiceParameters.Audit;
using Aro.Admin.Application.Services.LogManager;
using Aro.Admin.Application.Services.RequestInterpretor;
using Aro.Admin.Application.Services.Serializer;
using Aro.Admin.Application.Services.UniqueIdGenerator;
using Aro.Admin.Domain.Repository;
using Aro.Admin.Domain.Shared;
using Aro.Admin.Domain.Shared.Audit;
using Aro.Common.Application.Services.Audit;

namespace Aro.Admin.Infrastructure.Services;

public partial class AuditService
(
    IUniqueIdGenerator idGenerator, 
    IRequestInterpretorService requestInterpretor,
    AuditActions auditActions,
    EntityTypes auditEntityTypes,
    IRepositoryManager repository,
    ISerializer serializer,
    ILogManager<AuditService> logger
) : IAuditService
{
    public async Task LogApplicationSeeded(CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Starting {MethodName}", nameof(LogApplicationSeeded));
        
        var entity = GenerateAuditTrialEntityWithCommonParams
        (
            action: auditActions.ApplicationSeeded
        );

        logger.LogDebug("Generated audit entity for application seeded with action: {Action}", auditActions.ApplicationSeeded);
        
        await CreateTrial(entity, cancellationToken).ConfigureAwait(false);
        
        logger.LogDebug("Completed {MethodName}", nameof(LogApplicationSeeded));
    }

    public async Task LogMigrationsCompleted(CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Starting {MethodName}", nameof(LogMigrationsCompleted));
        
        var entity = GenerateAuditTrialEntityWithCommonParams
        (
            action: auditActions.MigrationsApplied
        );

        logger.LogDebug("Generated audit entity for migrations completed with action: {Action}", auditActions.MigrationsApplied);
        
        await CreateTrial(entity, cancellationToken).ConfigureAwait(false);
        
        logger.LogDebug("Completed {MethodName}", nameof(LogMigrationsCompleted));
    }

    public async Task LogSystemInitialized(SystemInitializedLog log, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Starting {MethodName}", nameof(LogSystemInitialized));
        
        var entity = GenerateAuditTrialEntityWithCommonParams
        (
            action: auditActions.SystemInitialized,
            //stateAfter: serializer.Serialize(log)
            data: serializer.Serialize(log)
        );

        logger.LogDebug("Generated audit entity for system initialized with action: {Action}, data: {Data}", auditActions.SystemInitialized, serializer.Serialize(log));
        
        await CreateTrial(entity, cancellationToken).ConfigureAwait(false);
        
        logger.LogDebug("Completed {MethodName}", nameof(LogSystemInitialized));
    }

    public async Task LogUserCreated(UserCreatedLog log, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Starting {MethodName}", nameof(LogUserCreated));
        
        var entity = GenerateTrailForUserCreated(auditActions.UserCreated, log.Id, log);

        logger.LogDebug("Generated audit entity for user created with action: {Action}, userId: {UserId}", auditActions.UserCreated, log.Id);
        
        await CreateTrial(entity, cancellationToken).ConfigureAwait(false);
        
        logger.LogDebug("Completed {MethodName}", nameof(LogUserCreated));
    }

    public async Task LogRolesAssigned(RolesAssignedLog log, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Starting {MethodName}", nameof(LogRolesAssigned));
        
        var entity = GenerateTrialForRolesAssigned(auditActions.RolesAssignedToUsers, log);

        logger.LogDebug("Generated audit entity for roles assigned with action: {Action}, userCount: {UserCount}, roleCount: {RoleCount}", auditActions.RolesAssignedToUsers, log.UserIds.Count, log.RoleIds.Count);
        
        await CreateTrial(entity, cancellationToken).ConfigureAwait(false);
        
        logger.LogDebug("Completed {MethodName}", nameof(LogRolesAssigned));
    }

    public async Task LogRolesRevoked(RolesRevokedLog log, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Starting {MethodName}", nameof(LogRolesRevoked));
        
        var entity = GenerateTrialForRolesRevoked(auditActions.RolesRevokedFromUsers, log);

        logger.LogDebug("Generated audit entity for roles revoked with action: {Action}, userCount: {UserCount}, roleCount: {RoleCount}", auditActions.RolesRevokedFromUsers, log.UserIds.Count, log.RoleIds.Count);
        
        await CreateTrial(entity, cancellationToken).ConfigureAwait(false);
        
        logger.LogDebug("Completed {MethodName}", nameof(LogRolesRevoked));
    }

    public async Task LogAuthenticationSuccessful(AuthenticationSuccessfulLog log, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Starting {MethodName}", nameof(LogAuthenticationSuccessful));
        
        var entity = GenerateTrialForAuthenticationSuccesful(log);

        logger.LogDebug("Generated audit entity for authentication successful with action: {Action}, email: {Email}", auditActions.AuthenticationSuccessful, log.Email);
        
        await CreateTrial(entity, cancellationToken).ConfigureAwait(false);
        
        logger.LogDebug("Completed {MethodName}", nameof(LogAuthenticationSuccessful));
    }

    public async Task LogAuthenticationFailed(AuthenticationFailedLog log, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Starting {MethodName}", nameof(LogAuthenticationFailed));
        
        var entity = GenerateTrialForAuthenticationFailed(log);

        logger.LogDebug("Generated audit entity for authentication failed with action: {Action}, email: {Email}", auditActions.AuthenticationFailed, log.Email);
        
        await CreateTrial(entity, cancellationToken).ConfigureAwait(false);
        
        logger.LogDebug("Completed {MethodName}", nameof(LogAuthenticationFailed));
    }

    public async Task LogUserSessionLoggedOutLog(UserSessionLoggedOutLog log, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Starting {MethodName}", nameof(LogUserSessionLoggedOutLog));
        
        var entity = GenerateTrailForUserSessionLoggedOut(log);

        logger.LogDebug("Generated audit entity for user session logged out with action: {Action}, refreshTokenHash: {RefreshTokenHash}", auditActions.UserSessionLoggedOut, log.RefreshTokenHash);
        
        await CreateTrial(entity, cancellationToken).ConfigureAwait(false);
        
        logger.LogDebug("Completed {MethodName}", nameof(LogUserSessionLoggedOutLog));
    }

    public async Task LogUserSessionsLoggedOutLog(UserSessionsLoggedOutLog log, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Starting {MethodName}", nameof(LogUserSessionsLoggedOutLog));
        
        var entity = GenerateTrailForUserSessionsLoggedOut(log);

        logger.LogDebug("Generated audit entity for user sessions logged out with action: {Action}", auditActions.UserSessionLoggedOut);
        
        await CreateTrial(entity, cancellationToken).ConfigureAwait(false);
        
        logger.LogDebug("Completed {MethodName}", nameof(LogUserSessionsLoggedOutLog));
    }

    public async Task LogTokenRefreshedLog(TokenRefreshedLog log, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Starting {MethodName}", nameof(LogTokenRefreshedLog));
        
        var entity = GenerateTrailForTokenRefreshed(log);

        logger.LogDebug("Generated audit entity for token refreshed with action: {Action}, oldRefreshTokenHash: {OldRefreshTokenHash}", auditActions.UserSessionLoggedOut, log.OldRefreshTokenHash);
        
        await CreateTrial(entity, cancellationToken).ConfigureAwait(false);
        
        logger.LogDebug("Completed {MethodName}", nameof(LogTokenRefreshedLog));
    }

    public async Task LogPasswordResetTokenGenerated(PasswordResetTokenGeneratedLog log, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Starting {MethodName}", nameof(LogPasswordResetTokenGenerated));
        
        var entity = GenerateTrailForPasswordResetTokenGenerated(log);

        logger.LogDebug("Generated audit entity for password reset token generation with action: {Action}, userId: {UserId}", 
            "password_reset_token_generated", log.UserId);

        await CreateTrial(entity, cancellationToken).ConfigureAwait(false);
        
        logger.LogDebug("Completed {MethodName}", nameof(LogPasswordResetTokenGenerated));
    }

    public async Task LogPasswordResetCompleted(PasswordResetCompletedLog log, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Starting {MethodName}", nameof(LogPasswordResetCompleted));
        
        var entity = GenerateTrailForPasswordResetCompleted(log);

        logger.LogDebug("Generated audit entity for password reset completed with action: {Action}, userId: {UserId}", 
            auditActions.PasswordResetCompleted, log.UserId);

        await CreateTrial(entity, cancellationToken).ConfigureAwait(false);
        
        logger.LogDebug("Completed {MethodName}", nameof(LogPasswordResetCompleted));
    }

    public async Task LogPasswordResetFailed(PasswordResetFailedLog log, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Starting {MethodName}", nameof(LogPasswordResetFailed));
        
        var entity = GenerateTrailForPasswordResetFailed(log);

        logger.LogDebug("Generated audit entity for password reset failed with action: {Action}, userId: {UserId}, reason: {Reason}", 
            auditActions.PasswordResetFailed, log.UserId?.ToString() ?? string.Empty, log.FailureReason);

        await CreateTrial(entity, cancellationToken).ConfigureAwait(false);
        
        logger.LogDebug("Completed {MethodName}", nameof(LogPasswordResetFailed));
    }

    public async Task LogPasswordResetLinkGenerated(PasswordResetLinkGeneratedLog log, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Starting {MethodName}", nameof(LogPasswordResetLinkGenerated));
        
        var entity = GenerateTrailForPasswordResetLinkGenerated(log);

        logger.LogDebug("Generated audit entity for password reset link generated with action: {Action}, email: {Email}", 
            auditActions.PasswordResetLinkGenerated, log.Email);

        await CreateTrial(entity, cancellationToken).ConfigureAwait(false);
        
        logger.LogDebug("Completed {MethodName}", nameof(LogPasswordResetLinkGenerated));
    }

    public async Task LogPasswordResetLinkGenerationFailed(PasswordResetLinkGenerationFailedLog log, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Starting {MethodName}", nameof(LogPasswordResetLinkGenerationFailed));
        
        var entity = GenerateTrailForPasswordResetLinkGenerationFailed(log);

        logger.LogDebug("Generated audit entity for password reset link generation failed with action: {Action}, email: {Email}, errorCode: {ErrorCode}", 
            auditActions.PasswordResetLinkGenerationFailed, log.Email, log.ErrorCode);

        await CreateTrial(entity, cancellationToken).ConfigureAwait(false);
        
        logger.LogDebug("Completed {MethodName}", nameof(LogPasswordResetLinkGenerationFailed));
    }

    public async Task LogPasswordChangedSuccess(ChangePasswordSuccessLog log, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Starting {MethodName}", nameof(LogPasswordChangedSuccess));

        var entity = GenerateTrailForPasswordChangeSuccess(log);

        logger.LogDebug("Generated audit entity for password change success with action: {Action}, email: {Email}",
            auditActions.PasswordResetLinkGenerationFailed, log.Email);

        await CreateTrial(entity, cancellationToken).ConfigureAwait(false);

        logger.LogDebug("Completed {MethodName}", nameof(LogPasswordChangedSuccess));
    }

    public async Task LogPasswordChangedFailed(ChangePasswordFailedLog log, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Starting {MethodName}", nameof(LogPasswordChangedFailed));

        var entity = GenerateTrailForPasswordChangeFailure(log);

        logger.LogDebug("Generated audit entity for password change failed with action: {Action}, email: {Email}, error: {ErrorMessage}",
            auditActions.PasswordResetLinkGenerationFailed, log.Email, log.ErrorMessage);

        await CreateTrial(entity, cancellationToken).ConfigureAwait(false);

        logger.LogDebug("Completed {MethodName}", nameof(LogPasswordResetLinkGenerationFailed));
    }

    public async Task LogGroupCreated(GroupCreatedLog log, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Starting {MethodName}", nameof(LogGroupCreated));

        var entity = GenerateTrailForGroupCreated(auditActions.GroupCreated, log.Id, log);

        logger.LogDebug("Generated audit entity for group created with action: {Action}, groupID: {GroupID}", auditActions.GroupCreated, log.Id);

        await CreateTrial(entity, cancellationToken).ConfigureAwait(false);

        logger.LogDebug("Completed {MethodName}", nameof(LogGroupCreated));
    }

    #region [Entity Patches] 

    public async Task LogGroupPatched(GroupPatchedLog log, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Starting {MethodName}", nameof(LogGroupPatched));

        var entity = GenerateTrailForGroupPatched(auditActions.GroupPatched, log.Id, log);

        logger.LogDebug("Generated audit entity for group patched with action: {Action}, groupID: {GroupID}", auditActions.GroupPatched, log.Id);

        await CreateTrial(entity, cancellationToken).ConfigureAwait(false);

        logger.LogDebug("Completed {MethodName}", nameof(LogGroupPatched));
    }

    #endregion

    #region [Entity Deletion]

    public async Task LogGroupDeleted(GroupDeletedLog log, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Starting {MethodName}", nameof(LogGroupDeleted));

        var entity = GenerateTrailForGroupDeleted(auditActions.GroupDeleted, log.Id, log);

        logger.LogDebug("Generated audit entity for group deleted with action: {Action}, groupID: {GroupID}", auditActions.GroupDeleted, log.Id);

        await CreateTrial(entity, cancellationToken).ConfigureAwait(false);

        logger.LogDebug("Completed {MethodName}", nameof(LogGroupDeleted));
    }

    #endregion
}
