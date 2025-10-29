using Aro.Admin.Domain.Entities;

namespace Aro.Admin.Domain.Repository;

public interface IPasswordHistoryRepository
{
	Task Create(PasswordHistory history, CancellationToken cancellationToken = default);

	IQueryable<PasswordHistory> GetByUserId(Guid userId);
}


