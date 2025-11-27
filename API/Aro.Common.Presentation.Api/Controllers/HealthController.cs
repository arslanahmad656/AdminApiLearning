using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;

namespace Aro.Common.Presentation.Api.Controllers;

[ApiController]
[Route("api/health")]
public class HealthController(IConfiguration configuration) : ControllerBase
{
    [HttpPost("environment")]
    public IActionResult GetEnvironment()
    {
        return Ok(new Dictionary<string, object?>
        {
            ["ASPNETCORE_ENVIRONMENT"] = configuration["ASPNETCORE_ENVIRONMENT"],
            ["DEFINE_CONFIG_ENVIRONMENT"] = Environment.GetEnvironmentVariable("DEFINE_CONFIG_ENVIRONMENT")
        });
    }
}
