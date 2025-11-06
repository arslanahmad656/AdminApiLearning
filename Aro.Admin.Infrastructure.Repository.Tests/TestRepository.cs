using Aro.Admin.Infrastructure.Repository.Context;
using Aro.Admin.Infrastructure.Repository.Repositories;
using Aro.Common.Domain.Entities;

namespace Aro.Admin.Infrastructure.Repository.Tests;

public class TestRepository(AroAdminApiDbContext dbContext) : RepositoryBase<User>(dbContext);