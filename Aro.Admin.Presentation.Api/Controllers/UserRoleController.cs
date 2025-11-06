using Aro.Admin.Application.Mediator.UserRole.Commands;
using Aro.Admin.Application.Mediator.UserRole.DTOs;
using Aro.Admin.Application.Mediator.UserRole.Queries;
using Aro.Admin.Domain.Shared;
using Aro.Admin.Presentation.Api.DTOs;
using Aro.Admin.Presentation.Api.Filters;
using Aro.Common.Application.Services.LogManager;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Aro.Admin.Presentation.Api.Controllers;

[ApiController]
[Route("api/userrole")]
public class UserRoleController(IMediator mediator, ILogManager<UserRoleController> logger) : ControllerBase
{
    [HttpPost("assignrolesbyids")]
    [Permissions(PermissionCodes.AssignUserRole)]
    public async Task<IActionResult> AssignRolesToUsers([FromBody] AssignRolesModel model, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Starting AssignRolesToUsers operation for userIds: {UserIds}, roleIds: {RoleIds}",
            string.Join(", ", model.UserIds), string.Join(", ", model.RoleIds));

        var response = await mediator.Send(new AssignRolesByIdCommand(new(model.UserIds, model.RoleIds)), cancellationToken).ConfigureAwait(false);

        logger.LogDebug("Completed AssignRolesToUsers operation successfully");
        return Ok(response);
    }

    [HttpPost("revokerolesbyids")]
    [Permissions(PermissionCodes.RevokeUserRole)]
    public async Task<IActionResult> RevokeRolesFromusers([FromBody] RevokeRolesModel model, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Starting RevokeRolesFromusers operation for userIds: {UserIds}, roleIds: {RoleIds}",
            string.Join(", ", model.UserIds), string.Join(", ", model.RoleIds));

        var response = await mediator.Send(new RevokeRolesByIdCommand(new(model.UserIds, model.RoleIds)), cancellationToken).ConfigureAwait(false);

        logger.LogDebug("Completed RevokeRolesFromusers operation successfully");
        return Ok(response);
    }

    [HttpGet("userhasrole")]
    [Permissions(PermissionCodes.TestUserRole)]
    public async Task<IActionResult> UserHasRole([FromQuery] Guid userId, [FromQuery] Guid roleId, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Starting UserHasRole operation for userId: {UserId}, roleId: {RoleId}", userId, roleId);

        var response = await mediator.Send(new UserHasRoleQuery(new UserHasRoleRequest(userId, roleId)), cancellationToken).ConfigureAwait(false);

        logger.LogDebug("Completed UserHasRole operation with result: {HasRole}", response);
        return Ok(response);
    }

    [HttpGet("userroles/{userId:Guid}")]
    [Permissions(PermissionCodes.GetUserRoles)]
    public async Task<IActionResult> GetUserRoles(Guid userId, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Starting GetUserRoles operation for userId: {UserId}", userId);

        var roles = await mediator.Send(new GetUserRolesQuery(new GetUserRolesRequest(userId)), cancellationToken).ConfigureAwait(false);

        logger.LogDebug("Completed GetUserRoles operation successfully");
        return Ok(roles);
    }
}
