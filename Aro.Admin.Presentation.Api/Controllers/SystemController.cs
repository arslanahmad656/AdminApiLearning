using Aro.Admin.Application.Mediator.Seed.Commands;
using Aro.Admin.Presentation.Api.Filters;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Aro.Admin.Domain.Shared.PermissionCodes;

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
    [Permissions(SeedApplication)]
    public async Task<IActionResult> Seed([FromBody] string jsonPath, CancellationToken cancellationToken)
    {
        await mediator.Send(new SeedApplicationCommand(jsonPath), cancellationToken).ConfigureAwait(false);
        return Ok();
    }
}
