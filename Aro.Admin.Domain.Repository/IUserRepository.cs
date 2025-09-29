using Aro.Admin.Domain.Entities;

namespace Aro.Admin.Domain.Repository;

public interface IUserRepository
{
    IQueryable<User> GetById(Guid id);

    Task<bool> UsersExist(CancellationToken cancellationToken = default);

    Task Create(User user, CancellationToken cancellationToken = default);
}
