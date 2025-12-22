using Aro.Common.Application.Shared;

namespace Aro.Admin.Application.Services.CountriesSeeder;

public interface ICountrySeeder : IService
{
    Task Seed(string jsonFile, CancellationToken cancellationToken = default);
}