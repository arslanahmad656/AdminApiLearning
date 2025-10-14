using Aro.Admin.Application.Mediator.User.Commands;
using Aro.Admin.Application.Services;
using Aro.Admin.Presentation.Api.DTOs;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MediatorDtos = Aro.Admin.Application.Mediator.User.DTOs;
using Aro.Admin.Presentation.Api.Filters;
using Aro.Admin.Domain.Shared;
using Aro.Admin.Application.Mediator.User.Queries;
using Microsoft.AspNetCore.Authorization;

namespace Aro.Admin.Presentation.Api.Controllers;

[ApiController]
[Route("api/user")]
public class UserController(IMediator mediator, IMapper mapper, ILogManager<UserController> logger) : ControllerBase
{
    [HttpPost("create")]
    [Permissions(PermissionCodes.CreateUser)]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserModel model, CancellationToken cancellationToken)
    {
        logger.LogDebug("Starting CreateUser operation for email: {Email}, displayName: {DisplayName}, isActive: {IsActive}, assignedRoles: {AssignedRoles}", 
            model.Email, model.DisplayName, model.IsActive, string.Join(", ", model.AssignedRoles));
        
        var response = await mediator.Send(new CreateUserCommand(mapper.Map<MediatorDtos.CreateUserRequest>(model)), cancellationToken).ConfigureAwait(false);

        logger.LogDebug("Completed CreateUser operation successfully");
        return Ok(response);
    }

    [HttpPost("getbootstrapuser")]
    [AllowAnonymous]
    public async Task<IActionResult> GetBootstrapUser([FromBody] GetBootstrapUserModel model, CancellationToken cancellationToken)
    {
        logger.LogDebug("Starting GetBootstrapUser operation.");

        var response = await mediator.Send(new GetBootstrapUserQuery(model.BootstrapPassword), cancellationToken).ConfigureAwait(false);

        logger.LogDebug("Completed GetBootstrapUser operation successfully.");
        return Ok(response);
    }
}
