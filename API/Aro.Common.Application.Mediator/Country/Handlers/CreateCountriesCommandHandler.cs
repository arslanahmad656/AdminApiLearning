using Aro.Common.Application.Mediator.Country.Commands;
using Aro.Common.Application.Mediator.Country.DTOs;
using Aro.Common.Application.Mediator.Country.Notifications;
using Aro.Common.Application.Services.Country;
using MediatR;

namespace Aro.Common.Application.Mediator.Country.Handlers;

public class CreateCountriesCommandHandler(ICountryService countryService, IMediator mediator) : IRequestHandler<CreateCountriesCommand, CreateCountriesResponse>
{

    public async Task<CreateCountriesResponse> Handle(CreateCountriesCommand request, CancellationToken cancellationToken)
    {
        var serviceResponse = await countryService.Create(request.Data.CountryRequests.Select(c => new CountryDto(c.Name, c.OfficialName, c.ISO2, c.PostalCodeRegex, c.PhoneCountryCode, c.PhoneNumberRegex)), cancellationToken);

        var resposne = new CreateCountriesResponse([.. serviceResponse]);

        await mediator.Publish(new CountriesCreatedNotification(new(resposne)), cancellationToken).ConfigureAwait(false);

        return resposne;
    }
}
