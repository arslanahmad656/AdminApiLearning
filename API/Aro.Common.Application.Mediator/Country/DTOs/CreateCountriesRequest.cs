namespace Aro.Common.Application.Mediator.Country.DTOs;

public record CreateCountriesRequest(IEnumerable<CreateCountryRequest> CountryRequests);
