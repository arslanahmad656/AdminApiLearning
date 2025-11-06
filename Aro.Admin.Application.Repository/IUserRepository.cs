using Aro.Admin.Domain.Entities;

namespace Aro.Admin.Application.Repository;

public interface IUserRepository
{
    IQueryable<User> GetById(Guid id);

    IQueryable<User> GetByEmail(string email);

    IQueryable<User> GetAll();

    Task<bool> UsersExist(CancellationToken cancellationToken = default);

    Task Create(User user, CancellationToken cancellationToken = default);

    void Update(User user);
}
