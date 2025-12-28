using Aro.Common.Domain.Entities;

namespace Aro.Common.Application.Repository;

public interface IUserRepository
{
    IQueryable<User> GetById(Guid id);

    IQueryable<User> GetByEmail(string email);

    IQueryable<User> GetAll();

    Task<bool> UsersExist(CancellationToken cancellationToken = default);

    Task Create(User user, CancellationToken cancellationToken = default);

    void Update(User user);

    void Delete(User user);
}
