using Aro.Common.Application.Services.Metadata;
using Microsoft.AspNetCore.Mvc;

namespace Aro.Common.Presentation.Api.Controllers;

[ApiController]
[Route("api/country-metadata")]
public class CountryMetadataController(ICountryMetadataService countryMetadataProvider) : ControllerBase
{

    [HttpGet]
    public IActionResult GetAllCountries()
    {
        return Ok(countryMetadataProvider.Countries);
    }
}
