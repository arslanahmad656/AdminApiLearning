using Aro.Admin.Domain.Entities;
using Aro.Admin.Domain.Repository;
using Aro.Admin.Infrastructure.Repository.Context;

namespace Aro.Admin.Infrastructure.Repository.Repositories;

internal class PasswordHistoryRepository(AroAdminApiDbContext dbContext)
	: RepositoryBase<PasswordHistory>(dbContext), IPasswordHistoryRepository
{
	public Task Create(PasswordHistory history, CancellationToken cancellationToken = default) => base.Add(history, cancellationToken);

	public IQueryable<PasswordHistory> GetByUserId(Guid userId) =>
		FindByCondition(filter: ph => ph.UserId == userId,
			orderBy: q => q.OrderByDescending(ph => ph.PasswordSetDate));
}


