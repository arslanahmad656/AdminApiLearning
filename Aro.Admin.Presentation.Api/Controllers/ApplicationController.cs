using Aro.Admin.Application.Mediator.Seed.Commands;
using Aro.Admin.Application.Mediator.SystemSettings.Commands;
using Aro.Admin.Application.Mediator.SystemSettings.DTOs;
using Aro.Admin.Application.Mediator.SystemSettings.Queries;
using Aro.Admin.Domain.Shared;
using Aro.Admin.Presentation.Api.DTOs;
using Aro.Admin.Presentation.Api.Filters;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Aro.Admin.Presentation.Api.Controllers;

[ApiController]
[Route("api/app")]
public class ApplicationController(IMediator mediator, IMapper mapper) : ControllerBase
{
    [HttpPost("reseed")]
    [Permissions(PermissionCodes.SeedApplication)]
    public async Task<IActionResult> Seed([FromBody] SeedModel model, CancellationToken cancellationToken)
    {
        await mediator.Send(new SeedApplicationCommand(model.FilePath), cancellationToken).ConfigureAwait(false);

        return Ok();
    }

    [HttpGet("isinitialized")]
    [Permissions(PermissionCodes.GetSystemSettings)]
    public async Task<IActionResult> IsApplicationInitialized(CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new IsSystemInitializedQuery(), cancellationToken).ConfigureAwait(false);

        return Ok(response);
    }

    [HttpPost("initialize")]
    [Permissions(PermissionCodes.InitializeSystem, PermissionCodes.CreateUser, PermissionCodes.AssignUserRole)]
    public async Task<IActionResult> InitializeSystem([FromBody] InitializeApplicationModel model, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new InitializeSystemCommand(mapper.Map<BootstrapUser>(model)), cancellationToken).ConfigureAwait(false);

        return Ok(response);
    }
}
