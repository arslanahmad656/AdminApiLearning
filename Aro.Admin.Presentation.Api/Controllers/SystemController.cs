using Aro.Admin.Application.Mediator.Seed.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Aro.Admin.Presentation.Api.Controllers;

[ApiController]
[Route("api/system")]
public class SystemController
(
    IMediator mediator
)
: ControllerBase
{
    [HttpPost("seed")]
    [Authorize(Roles = "SystemAdmin")]
    public async Task<IActionResult> Seed([FromBody] string jsonPath, CancellationToken cancellationToken)
    {
        await mediator.Send(new SeedApplicationCommand(jsonPath), cancellationToken).ConfigureAwait(false);
        return Ok();
    }
}
