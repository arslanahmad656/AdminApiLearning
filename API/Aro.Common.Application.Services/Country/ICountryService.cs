using System.Diagnostics.Metrics;

namespace Aro.Common.Application.Services.Country;

public interface ICountryService
{
    Task<List<CountryResponse>> GetAll(CancellationToken cancellationToken = default);

    Task<List<Guid>> Create(IEnumerable<CountryDto> countries, CancellationToken cancellationToken = default);
}
