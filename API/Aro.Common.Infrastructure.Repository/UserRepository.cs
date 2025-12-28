using Aro.Common.Application.Repository;
using Aro.Common.Domain.Entities;
using Aro.Common.Infrastructure.Repository.Context;
using Microsoft.EntityFrameworkCore;

namespace Aro.Common.Infrastructure.Repository;

public class UserRepository(AroDbContext dbContext) : RepositoryBase<User>(dbContext), IUserRepository
{
    public IQueryable<User> GetById(Guid id) => FindByCondition(filter: u => u.Id == id);

    public IQueryable<User> GetByEmail(string email) => FindByCondition(filter: u => u.Email == email);

    public IQueryable<User> GetAll() => FindByCondition();

    public Task<bool> UsersExist(CancellationToken cancellationToken = default) => FindByCondition()
        .AnyAsync(cancellationToken);

    public Task Create(User user, CancellationToken cancellationToken = default) => Add(user, cancellationToken);

    public new void Update(User user) => base.Update(user);

    public new void Delete(User user) => base.Delete(user);
}