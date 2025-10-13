using Aro.Admin.Application.Mediator.Seed.Commands;
using Aro.Admin.Application.Services;
using Aro.Admin.Presentation.Api.DTOs;
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
    IMediator mediator,
    ILogManager<SystemController> logger
)
: ControllerBase
{
    [HttpPost("seed")]
    [Permissions(SeedApplication)]
    public async Task<IActionResult> Seed([FromBody] SeedModel model, CancellationToken cancellationToken)
    {
        logger.LogDebug("Starting Seed operation with FilePath: {FilePath}", model.FilePath);
        
        await mediator.Send(new SeedApplicationCommand(model.FilePath), cancellationToken).ConfigureAwait(false);
        
        logger.LogDebug("Completed Seed operation successfully");
        return Ok();
    }
}
