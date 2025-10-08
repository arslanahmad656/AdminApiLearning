using Aro.Admin.Application.Mediator.User.Commands;
using Aro.Admin.Presentation.Api.DTOs;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MediatorDtos = Aro.Admin.Application.Mediator.User.DTOs;
using Aro.Admin.Presentation.Api.Filters;
using Aro.Admin.Domain.Shared;

namespace Aro.Admin.Presentation.Api.Controllers;

[ApiController]
[Route("api/user")]
public class UserController(IMediator mediator, IMapper mapper) : ControllerBase
{
    [HttpPost("create")]
    [Permissions(PermissionCodes.CreateUser)]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserModel model, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new CreateUserCommand(mapper.Map<MediatorDtos.CreateUserRequest>(model)), cancellationToken).ConfigureAwait(false);

        return CreatedAtAction("TODO", response);
    }
}
