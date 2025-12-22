using Aro.Common.Application.Mediator.Country.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Aro.Common.Presentation.Api.Controllers;

[ApiController]
[Route("api/country-metadata")]
public class CountryController(IMediator mediator) : ControllerBase
{

    [HttpGet]
    public async Task<IActionResult> GetAllCountries(CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetAllCountriesQuery(), cancellationToken).ConfigureAwait(false);

        return Ok(response);
    }
}
