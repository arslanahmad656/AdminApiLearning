using Aro.Admin.Domain.Entities;

namespace Aro.Admin.Domain.Repository;

public interface IUserRepository
{
    Task<bool> UsersExist(CancellationToken cancellationToken = default);

    Task Create(User user, CancellationToken cancellationToken = default);
}
