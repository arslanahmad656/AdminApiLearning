using Aro.Admin.Application.Mediator.UserRole.Commands;
using Aro.Admin.Application.Mediator.UserRole.DTOs;
using Aro.Admin.Application.Mediator.UserRole.Queries;
using Aro.Admin.Domain.Shared;
using Aro.Admin.Presentation.Api.DTOs;
using Aro.Admin.Presentation.Api.Filters;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Aro.Admin.Presentation.Api.Controllers;

[ApiController]
[Route("api/userrole")]
public class UserRoleController(IMediator mediator, IMapper mapper) : ControllerBase
{
    [HttpPost("assignrolesbyids")]
    [Permissions(PermissionCodes.AssignUserRole)]
    public async Task<IActionResult> AssignRolesToUsers([FromBody] AssignRolesModel model, CancellationToken cancellationToken = default)
    {
        var response = await mediator.Send(new AssignRolesByIdCommand(mapper.Map<AssignRolesByIdRequest>(model)), cancellationToken).ConfigureAwait(false);

        return Ok(response);
    }

    [HttpPost("revokerolesbyids")]
    [Permissions(PermissionCodes.RevokeUserRole)]
    public async Task<IActionResult> RevokeRolesFromusers([FromBody] RevokeRolesModel model, CancellationToken cancellationToken = default)
    {
        var response = await mediator.Send(new RevokeRolesByIdCommand(mapper.Map<RevokeRolesByIdRequest>(model)), cancellationToken).ConfigureAwait(false);

        return Ok(response);
    }

    [HttpGet("userhasrole")]
    [Permissions(PermissionCodes.TestUserRole)]
    public async Task<IActionResult> UserHasRole([FromQuery] Guid userId, [FromQuery] Guid roleId, CancellationToken cancellationToken = default)
    {
        var response = await mediator.Send(new UserHasRoleQuery(new(userId, roleId)), cancellationToken).ConfigureAwait(false);

        return Ok(response);
    }

    [HttpGet("userroles/{userId:Guid}")]
    [Permissions(PermissionCodes.GetUserRoles)]
    public async Task<IActionResult> GetUserRoles(Guid userId, CancellationToken cancellationToken = default)
    {
        var roles = await mediator.Send(new GetUserRolesQuery(new(userId)), cancellationToken).ConfigureAwait(false);

        return Ok(roles);
    }
}
