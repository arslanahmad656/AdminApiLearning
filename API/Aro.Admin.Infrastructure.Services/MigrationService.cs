using Aro.Admin.Application.Services.Migration;
using Aro.Common.Application.Services.Authorization;
using Aro.Common.Application.Services.LogManager;
using Aro.Common.Domain.Shared;
using Aro.Common.Infrastructure.Repository.Context;
using Microsoft.EntityFrameworkCore;

namespace Aro.Admin.Infrastructure.Services;

public class MigrationService(AroDbContext context, IAuthorizationService authorizationService, ILogManager<MigrationService> logger) : IMigrationService
{
    public async Task Migrate(bool dropFirst, CancellationToken cancellationToken)
    {
        logger.LogDebug("Starting {MethodName}.", nameof(Migrate));

        logger.LogDebug("Ensuring current user has migration permissions");
        await authorizationService.EnsureCurrentUserPermissions([PermissionCodes.MigrateDabase], cancellationToken).ConfigureAwait(false);
        logger.LogDebug("Migration permissions verified");

        logger.LogDebug("Executing database migration");
        if (dropFirst)
        {
            logger.LogWarn($"The existing database will be dropped first.");
            await context.Database.EnsureDeletedAsync(cancellationToken).ConfigureAwait(false);
            logger.LogWarn("Database dropped.");
        }

        logger.LogDebug("Applying pending migrations if any...");
        await context.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);
        logger.LogInfo("Database migration completed successfully");

        logger.LogDebug("Completed {MethodName}", nameof(Migrate));
    }
}
