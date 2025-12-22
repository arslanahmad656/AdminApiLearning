using Aro.Common.Domain.Entities;

namespace Aro.Common.Application.Repository;

public interface ICountryRepository
{
    IQueryable<Country> GetAll();

    Task Create(Country country, CancellationToken cancellationToken = default);
}
