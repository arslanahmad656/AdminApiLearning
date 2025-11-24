using Aro.Common.Application.Shared.Metadata;
using Microsoft.AspNetCore.Mvc;

namespace Aro.Common.Presentation.Api;

[ApiController]
[Route("api/country-metadata")]
public class CountryMetadataController(ICountryMetadataProvider countryMetadataProvider) : ControllerBase
{

    [HttpGet]
    public IActionResult GetAllCountries()
    {
        return Ok(countryMetadataProvider.Countries);
    }
}
