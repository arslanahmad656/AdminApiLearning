using Aro.Admin.Domain.Entities;
using Aro.Admin.Domain.Repository;
using Aro.Admin.Infrastructure.Repository.Context;
using Microsoft.EntityFrameworkCore;

namespace Aro.Admin.Infrastructure.Repository.Repositories;

public class UserRepository(AroAdminApiDbContext dbContext) : RepositoryBase<User>(dbContext), IUserRepository
{
    public IQueryable<User> GetById(Guid id) => FindByCondition(filter: u => u.Id == id);

    public IQueryable<User> GetByEmail(string email) => FindByCondition(filter: u => u.Email == email);
    public Task<bool> UsersExist(CancellationToken cancellationToken = default) => FindByCondition()
        .AnyAsync(cancellationToken);

    public Task Create(User user, CancellationToken cancellationToken = default) => base.Add(user, cancellationToken);
}