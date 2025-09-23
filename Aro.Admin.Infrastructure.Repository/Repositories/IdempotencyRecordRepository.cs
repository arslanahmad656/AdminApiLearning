using Aro.Admin.Domain.Entities;
using Aro.Admin.Domain.Repository;
using Aro.Admin.Infrastructure.Repository.Context;

namespace Aro.Admin.Infrastructure.Repository.Repositories;

public class IdempotencyRecordRepository(AroAdminApiDbContext dbContext) : RepositoryBase<IdempotencyRecord>(dbContext), IIdempotencyRecordRepository
{
    // Implement IIdempotencyRecordRepository members here
}