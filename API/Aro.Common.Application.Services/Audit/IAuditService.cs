using Aro.Common.Application.Shared;

namespace Aro.Common.Application.Services.Audit;

public interface IAuditService : IService
{
    //Task LogApplicationSeeded(CancellationToken cancellationToken = default);

    //Task LogMigrationsCompleted(CancellationToken cancellationToken = default);

    //Task LogSystemInitialized(SystemInitializedLog log, CancellationToken cancellationToken = default);

    //Task LogUserCreated(UserCreatedLog log, CancellationToken cancellationToken = default);

    //Task LogRolesAssigned(RolesAssignedLog log, CancellationToken cancellationToken = default);

    //Task LogRolesRevoked(RolesRevokedLog log, CancellationToken cancellationToken = default);

    //Task LogAuthenticationSuccessful(AuthenticationSuccessfulLog log, CancellationToken cancellationToken = default);

    //Task LogAuthenticationFailed(AuthenticationFailedLog log, CancellationToken cancellationToken = default);

    //Task LogUserSessionLoggedOutLog(UserSessionLoggedOutLog log, CancellationToken cancellationToken = default);

    //Task LogUserSessionsLoggedOutLog(UserSessionsLoggedOutLog log, CancellationToken cancellationToken = default);

    //Task LogTokenRefreshedLog(TokenRefreshedLog log, CancellationToken cancellationToken = default);

    //Task LogPasswordResetTokenGenerated(PasswordResetTokenGeneratedLog log, CancellationToken cancellationToken = default);

    //Task LogPasswordResetCompleted(PasswordResetCompletedLog log, CancellationToken cancellationToken = default);

    //Task LogPasswordResetFailed(PasswordResetFailedLog log, CancellationToken cancellationToken = default);

    //Task LogPasswordResetLinkGenerated(PasswordResetLinkGeneratedLog log, CancellationToken cancellationToken = default);

    //Task LogPasswordResetLinkGenerationFailed(PasswordResetLinkGenerationFailedLog log, CancellationToken cancellationToken = default);
    //Task LogPasswordChangedSuccess(ChangePasswordSuccessLog log, CancellationToken cancellationToken = default);

    //Task LogPasswordChangedFailed(ChangePasswordFailedLog log, CancellationToken cancellationToken = default);

    //Task LogGroupCreated(GroupCreatedLog log, CancellationToken cancellationToken = default);

    //Task LogGroupPatched(GroupPatchedLog log, CancellationToken cancellationToken = default);

    //Task LogGroupDeleted(GroupDeletedLog log, CancellationToken cancellationToken = default);

    Task Log(AuditEntryDto log, CancellationToken cancellationToken = default);

}
