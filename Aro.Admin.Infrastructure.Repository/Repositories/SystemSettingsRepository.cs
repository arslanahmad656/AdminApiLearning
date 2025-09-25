using Aro.Admin.Domain.Entities;
using Aro.Admin.Domain.Repository;
using Aro.Admin.Infrastructure.Repository.Context;
using Microsoft.EntityFrameworkCore;

namespace Aro.Admin.Infrastructure.Repository.Repositories;

public class SystemSettingsRepository(AroAdminApiDbContext dbContext) : RepositoryBase<SystemSetting>(dbContext), ISystemSettingsRepository
{
    public Task<SystemSetting?> GetValue(string key, CancellationToken cancellationToken = default)
        => FindByCondition(filter: s => s.Key == key)
            .FirstOrDefaultAsync(cancellationToken);
            

    public void UpdateSetting(SystemSetting setting) => base.Update(setting);

}