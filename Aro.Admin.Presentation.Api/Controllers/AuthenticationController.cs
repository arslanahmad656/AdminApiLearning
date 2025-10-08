using Aro.Admin.Application.Mediator.Authentication.Commands;
using Aro.Admin.Application.Services;
using Aro.Admin.Presentation.Api.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Aro.Admin.Presentation.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthenticationController(IMediator mediator) : ControllerBase
{
    [HttpPost("authenticate")]
    public async Task<IActionResult> Authenticate([FromBody] AuthenticationModel model, CancellationToken token)
    {
        var response = await mediator.Send(new AuthenticateUserCommand(new(model.Email, model.Password)), token).ConfigureAwait(false);

        return Ok(new
        {
            response.Expiry,
            AccessToken = response.Token
        });
    }
}
