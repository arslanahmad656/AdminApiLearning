using Aro.Admin.Application.Services.DTOs.ServiceParameters.Audit;

namespace Aro.Admin.Application.Services.DataServices;

public interface IAuditService
{
    Task LogApplicationSeeded(CancellationToken cancellationToken = default);

    Task LogMigrationsCompleted(CancellationToken cancellationToken = default);

    Task LogSystemInitialized(SystemInitializedLog log, CancellationToken cancellationToken = default);

    Task LogUserCreated(UserCreatedLog log, CancellationToken cancellationToken = default);

    Task LogRolesAssigned(RolesAssignedLog log, CancellationToken cancellationToken = default);

    Task LogRolesRevoked(RolesRevokedLog log, CancellationToken cancellationToken = default);

    Task LogAuthenticationSuccessful(AuthenticationSuccessfulLog log, CancellationToken cancellationToken = default);

    Task LogAuthenticationFailed(AuthenticationFailedLog log, CancellationToken cancellationToken = default);

    Task LogUserSessionLoggedOutLog(UserSessionLoggedOutLog log, CancellationToken cancellationToken = default);
    
    Task LogUserSessionsLoggedOutLog(UserSessionsLoggedOutLog log, CancellationToken cancellationToken = default);

    Task LogTokenRefreshedLog(TokenRefreshedLog log, CancellationToken cancellationToken = default);
}
