using Aro.Admin.Application.Services;
using Aro.Admin.Application.Services.DataServices;
using Aro.Admin.Domain.Repository;
using Aro.Admin.Domain.Shared.Audit;

namespace Aro.Admin.Infrastructure.Services;

public partial class AuditService
(
    IEntityIdGenerator idGenerator, 
    IRequestInterpretorService requestInterpretor,
    AuditActions auditActions,
    IRepositoryManager repository
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
}
