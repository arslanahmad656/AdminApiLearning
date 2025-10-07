using Aro.Admin.Application.Mediator.UserRole.Commands;
using Aro.Admin.Application.Mediator.UserRole.DTOs;
using Aro.Admin.Presentation.Api.DTOs;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Aro.Admin.Presentation.Api.Controllers;

[ApiController]
[Route("api/userrole")]
public class UserRoleController(IMediator mediator, IMapper mapper) : ControllerBase
{
    [HttpPost("assignrolesbyids")]
    public async Task<IActionResult> AssignRolesToUsers([FromBody] AssignRolesModel model, CancellationToken cancellationToken = default)
    {
        var response = await mediator.Send(new AssignRolesByIdCommand(mapper.Map<AssignRolesByIdRequest>(model)), cancellationToken).ConfigureAwait(false);

        return Ok(response);
    }

    [HttpPost("revokerolesbyids")]
    public async Task<IActionResult> RevokeRolesFromusers([FromBody] RevokeRolesModel model, CancellationToken cancellationToken = default)
    {
        var response = await mediator.Send(new RevokeRolesByIdCommand(mapper.Map<RevokeRolesByIdRequest>(model)), cancellationToken).ConfigureAwait(false);

        return Ok(response);
    }
}
