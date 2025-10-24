using Aro.Admin.Application.Mediator.Seed.Commands;
using Aro.Admin.Application.Mediator.SystemSettings.Commands;
using Aro.Admin.Application.Mediator.SystemSettings.DTOs;
using Aro.Admin.Application.Mediator.SystemSettings.Queries;
using Aro.Admin.Application.Services;
using Aro.Admin.Domain.Shared;
using Aro.Admin.Presentation.Api.DTOs;
using Aro.Admin.Presentation.Api.Filters;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Aro.Admin.Presentation.Api.Controllers;

[ApiController]
[Route("api/app")]
public class ApplicationController(IMediator mediator, ILogManager<ApplicationController> logger) : ControllerBase
{
    //[HttpPost("reseed")]
    //[Permissions(PermissionCodes.SeedApplication)]
    //public async Task<IActionResult> Seed([FromBody] SeedModel model, CancellationToken cancellationToken)
    //{
    //    logger.LogDebug("Starting Seed operation with FilePath: {FilePath}", model.FilePath);

    //    await mediator.Send(new SeedApplicationCommand(model.FilePath), cancellationToken).ConfigureAwait(false);

    //    logger.LogDebug("Completed Seed operation successfully");
    //    return Ok();
    //}

    [HttpGet("isinitialized")]
    [Permissions(PermissionCodes.GetSystemSettings)]
    public async Task<IActionResult> IsApplicationInitialized(CancellationToken cancellationToken)
    {
        logger.LogDebug("Starting IsApplicationInitialized operation");

        var response = await mediator.Send(new IsSystemInitializedQuery(), cancellationToken).ConfigureAwait(false);

        logger.LogDebug("Completed IsApplicationInitialized operation with result: {IsInitialized}", response);
        return Ok(response);
    }

    [HttpPost("initialize")]
    [AllowAnonymous]
    public async Task<IActionResult> InitializeSystem([FromBody] InitializeApplicationModel model, CancellationToken cancellationToken)
    {
        logger.LogDebug("Starting InitializeSystem operation for email: {Email}, displayName: {DisplayName}", model.Email, model.DisplayName);

        var response = await mediator.Send(new InitializeSystemCommand(new(model.Email, model.Password, model.DisplayName, model.BootstrapPassword)), cancellationToken).ConfigureAwait(false);

        logger.LogDebug("Completed InitializeSystem operation successfully");
        return Ok(response);
    }
}
