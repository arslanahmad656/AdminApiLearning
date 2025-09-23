using Aro.Admin.Domain.Entities;
using Aro.Admin.Domain.Repository;
using Aro.Admin.Infrastructure.Repository.Context;

namespace Aro.Admin.Infrastructure.Repository.Repositories;

public class UserRepository(AroAdminApiDbContext dbContext) : RepositoryBase<User>(dbContext), IUserRepository
{
}