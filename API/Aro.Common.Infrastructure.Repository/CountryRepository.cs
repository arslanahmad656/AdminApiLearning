using Aro.Common.Application.Repository;
using Aro.Common.Domain.Entities;
using Aro.Common.Infrastructure.Repository.Context;

namespace Aro.Common.Infrastructure.Repository;

public class CountryRepository(AroDbContext dbContext) : RepositoryBase<Country>(dbContext), ICountryRepository
{
    public Task Create(Country country, CancellationToken cancellationToken = default) => base.Add(country, cancellationToken);

    public IQueryable<Country> GetAll() => FindByCondition();
}
