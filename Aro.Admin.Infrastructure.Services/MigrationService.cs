using Aro.Admin.Application.Services;
using Aro.Admin.Domain.Shared;
using Aro.Admin.Infrastructure.Repository.Context;
using Microsoft.EntityFrameworkCore;

namespace Aro.Admin.Infrastructure.Services;

public class MigrationService(AroAdminApiDbContext context, IAuthorizationService authorizationService, PermissionCodes permissionCodes) : IMigrationService
{
    public async Task Migrate(CancellationToken cancellationToken)
    {
        await authorizationService.EnsureCurrentUserPermissions([permissionCodes.MigrateDabase], cancellationToken).ConfigureAwait(false);
        await context.Database.MigrateAsync(cancellationToken);
    }
}
