using Aro.Common.Application.Repository;
using Aro.Common.Infrastructure.Repository.Context;

namespace Aro.Common.Infrastructure.Repository;

public class UnitOfWork(AroDbContext dbContext) : IUnitOfWork
{
    public async Task SaveChanges(CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
