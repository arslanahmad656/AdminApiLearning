using Aro.Admin.Domain.Entities;
using Aro.Admin.Infrastructure.Repository.Context;
using Aro.Admin.Infrastructure.Repository.Repositories;

namespace Aro.Admin.Infrastructure.Repository.Tests;

public class TestRepository(AroAdminApiDbContext dbContext) : RepositoryBase<User>(dbContext);