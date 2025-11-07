using Aro.Admin.Application.Repository;
using Aro.Admin.Domain.Entities;
using Aro.Common.Infrastructure.Repository;
using Aro.Common.Infrastructure.Repository.Context;

namespace Aro.Admin.Infrastructure.Repository;

internal class PasswordHistoryRepository(AroDbContext dbContext)
    : RepositoryBase<PasswordHistory>(dbContext), IPasswordHistoryRepository
{
    public Task Create(PasswordHistory history, CancellationToken cancellationToken = default) => base.Add(history, cancellationToken);

    public IQueryable<PasswordHistory> GetByUserId(Guid userId) =>
        FindByCondition(filter: ph => ph.UserId == userId,
            orderBy: q => q.OrderByDescending(ph => ph.PasswordSetDate));

    public new void Delete(PasswordHistory history) => base.Delete(history);

    public new void DeleteRange(IEnumerable<PasswordHistory> histories) => base.DeleteRange(histories);
}


