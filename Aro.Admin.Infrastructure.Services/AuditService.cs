using Aro.Admin.Application.Services;
using Aro.Admin.Application.Services.DataServices;
using Aro.Admin.Application.Services.DTOs.ServiceParameters.Audit;
using Aro.Admin.Domain.Repository;
using Aro.Admin.Domain.Shared.Audit;

namespace Aro.Admin.Infrastructure.Services;

public partial class AuditService
(
    IEntityIdGenerator idGenerator, 
    IRequestInterpretorService requestInterpretor,
    AuditActions auditActions,
    AuditEntityTypes auditEntityTypes,
    IRepositoryManager repository,
    ISerializer serializer
) : IAuditService
{
    public async Task LogApplicationSeeded(CancellationToken cancellationToken = default)
    {
        var entity = GenerateAuditTrialEntityWithCommonParams
        (
            action: auditActions.ApplicationSeeded
        );

        await CreateTrial(entity, cancellationToken).ConfigureAwait(false);
    }

    public async Task LogMigrationsCompleted(CancellationToken cancellationToken = default)
    {
        var entity = GenerateAuditTrialEntityWithCommonParams
        (
            action: auditActions.MigrationsApplied
        );

        await CreateTrial(entity, cancellationToken).ConfigureAwait(false);
    }

    public async Task LogSystemInitialized(SystemInitializedLog log, CancellationToken cancellationToken = default)
    {
        var entity = GenerateAuditTrialEntityWithCommonParams
        (
            action: auditActions.SystemInitialized,
            //stateAfter: serializer.Serialize(log)
            data: serializer.Serialize(log)
        );

        await CreateTrial(entity, cancellationToken).ConfigureAwait(false);
    }

    public async Task LogUserCreated(UserCreatedLog log, CancellationToken cancellationToken = default)
    {
        var entity = GenerateTrailForUserCreated(auditActions.UserCreated, log.Id, log);

        await CreateTrial(entity, cancellationToken).ConfigureAwait(false);
    }

    public async Task LogRolesAssigned(RolesAssignedLog log, CancellationToken cancellationToken = default)
    {
        var entity = GenerateTrialForRolesAssigned(auditActions.RolesAssignedToUsers, log);

        await CreateTrial(entity, cancellationToken).ConfigureAwait(false);
    }

    public async Task LogRolesRevoked(RolesRevokedLog log, CancellationToken cancellationToken = default)
    {
        var entity = GenerateTrialForRolesRevoked(auditActions.RolesRevokedFromUsers, log);

        await CreateTrial(entity, cancellationToken).ConfigureAwait(false);
    }
}
