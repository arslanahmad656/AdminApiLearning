using Aro.Admin.Domain.Entities;
using Aro.Admin.Domain.Repository;
using Aro.Admin.Infrastructure.Repository.Context;

namespace Aro.Admin.Infrastructure.Repository.Repositories;

public class UserRoleRepository(AroAdminApiDbContext dbContext) : RepositoryBase<UserRole>(dbContext), IUserRoleRepository
{
    // Implement IUserRoleRepository members here
}