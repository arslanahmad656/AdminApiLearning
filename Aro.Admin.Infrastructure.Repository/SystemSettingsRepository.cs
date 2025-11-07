using Aro.Admin.Application.Repository;
using Aro.Admin.Domain.Entities;
using Aro.Common.Infrastructure.Repository;
using Aro.Common.Infrastructure.Repository.Context;
using Microsoft.EntityFrameworkCore;

namespace Aro.Admin.Infrastructure.Repository;

public class SystemSettingsRepository(AroDbContext dbContext) : RepositoryBase<SystemSetting>(dbContext), ISystemSettingsRepository
{
    public Task<SystemSetting?> GetValue(string key, CancellationToken cancellationToken = default)
        => FindByCondition(filter: s => s.Key == key)
            .FirstOrDefaultAsync(cancellationToken);


    public void UpdateSetting(SystemSetting setting) => base.Update(setting);

}