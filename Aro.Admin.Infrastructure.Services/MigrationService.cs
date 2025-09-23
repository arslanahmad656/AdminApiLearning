using Aro.Admin.Application.Services;
using Aro.Admin.Infrastructure.Repository.Context;
using Microsoft.EntityFrameworkCore;

namespace Aro.Admin.Infrastructure.Services;

public class MigrationService(AroAdminApiDbContext context) : IMigrationService
{
    public async Task Migrate(CancellationToken cancellationToken)
    {
        await context.Database.MigrateAsync(cancellationToken);
    }
}
