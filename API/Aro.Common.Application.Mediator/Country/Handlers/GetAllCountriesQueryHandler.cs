using Aro.Common.Application.Mediator.Country.DTOs;
using Aro.Common.Application.Mediator.Country.Queries;
using Aro.Common.Application.Services.Country;
using MediatR;

namespace Aro.Common.Application.Mediator.Country.Handlers;

public class GetAllCountriesQueryHandler(ICountryService countryService) : IRequestHandler<GetAllCountriesQuery, GetAllCountriesResponse>
{
    public async Task<GetAllCountriesResponse> Handle(GetAllCountriesQuery request, CancellationToken cancellationToken)
    {
        var serviceResponse = await countryService.GetAll(cancellationToken).ConfigureAwait(false);

        var response = new GetAllCountriesResponse([.. serviceResponse.Select(c => new GetCountryResponse(c.Id, c.Name, c.OfficialName, c.ISO2, c.PostalCodeRegex, c.PhoneCountryCode, c.PhoneNumberRegex))]);

        return response;
    }
}
