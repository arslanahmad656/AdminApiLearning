using Aro.Admin.Application.Mediator.Authentication.Commands;
using Aro.Admin.Application.Mediator.Authentication.DTOs;
using Aro.Admin.Application.Services;
using Aro.Admin.Presentation.Api.DTOs;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Aro.Admin.Presentation.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthenticationController(IMediator mediator, IMapper mapper) : ControllerBase
{
    [HttpPost("authenticate")]
    [AllowAnonymous]
    public async Task<IActionResult> Authenticate([FromBody] AuthenticationModel model, CancellationToken token)
    {
        var response = await mediator.Send(new AuthenticateUserCommand(new(model.Email, model.Password)), token).ConfigureAwait(false);

        return Ok(new
        {
            response.RefreshTokenExpiry,
            response.AccessToken
        });
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenModel model, CancellationToken cancellationToken)
    {
        var token = await mediator.Send(new RefreshTokenCommand(mapper.Map<RefreshTokenRequest>(model)), cancellationToken).ConfigureAwait(false);

        return Ok(new
        {
            token.AccessToken,
            token.RefreshToken,
            token.RefreshTokenExpiry,
            token.AccessTokenExpiry
        });
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout([FromBody] LogoutUserModel model, CancellationToken cancellationToken)
    {
        _ = await mediator.Send(new LogoutUserCommand(mapper.Map<LogoutUserRequest>(model)), cancellationToken).ConfigureAwait(false);

        return NoContent();
    }

    [HttpPost("logoutall")]
    [Authorize]
    public async Task<IActionResult> LogoutAll([FromBody] LogoutUserAllCommand model, CancellationToken cancellationToken)
    {
        await mediator.Send(new LogoutUserAllCommand(mapper.Map<LogoutUserAllRequest>(model)), cancellationToken).ConfigureAwait(false);

        return NoContent();
    }
}
