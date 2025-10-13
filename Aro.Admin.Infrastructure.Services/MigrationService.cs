using Aro.Admin.Application.Services;
using Aro.Admin.Application.Services.DataServices;
using Aro.Admin.Domain.Shared;
using Aro.Admin.Domain.Shared.Exceptions;
using Aro.Admin.Infrastructure.Repository.Context;
using Microsoft.EntityFrameworkCore;

namespace Aro.Admin.Infrastructure.Services;

public class MigrationService(AroAdminApiDbContext context, IAuthorizationService authorizationService, ILogManager<MigrationService> logger) : IMigrationService
{
    public async Task Migrate(CancellationToken cancellationToken)
    {
        logger.LogDebug("Starting {MethodName}.", nameof(Migrate));

        logger.LogDebug("Ensuring current user has migration permissions");
        await authorizationService.EnsureCurrentUserPermissions([PermissionCodes.MigrateDabase], cancellationToken).ConfigureAwait(false);
        logger.LogDebug("Migration permissions verified");
        
        logger.LogDebug("Executing database migration");
        await context.Database.MigrateAsync(cancellationToken);
        logger.LogInfo("Database migration completed successfully");
        
        logger.LogDebug("Completed {MethodName}", nameof(Migrate));
    }
}
