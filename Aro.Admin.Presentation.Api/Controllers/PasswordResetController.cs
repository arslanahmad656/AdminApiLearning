using Aro.Admin.Application.Mediator.PasswordReset.Commands;
using Aro.Admin.Application.Mediator.PasswordReset.DTOs;
using Aro.Admin.Presentation.Api.DTOs;
using Aro.Common.Application.Services.LogManager;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Aro.Admin.Presentation.Api.Controllers;

[ApiController]
[Route("api/password-reset")]
public class PasswordResetController(IMediator mediator, ILogManager<PasswordResetController> logger) : ControllerBase
{
    [HttpPost("send-reset-link")]
    public async Task<IActionResult> SendPasswordResetLink([FromBody] SendPasswordResetLinkModel model, CancellationToken cancellationToken)
    {
        logger.LogDebug("Starting SendPasswordResetLink operation for email: {Email}", model.Email);
        _ = await mediator.Send(new SendPasswordResetEmailLinkCommand(new(model.Email)), cancellationToken).ConfigureAwait(false);
        logger.LogDebug("Completed SendPasswordResetLink operation successfully");
        return NoContent(); // Security: The client should not know about the outcome.
    }

    [HttpPost("reset")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model, CancellationToken cancellationToken)
    {
        logger.LogDebug("Starting ResetPassword operation");

        var response = await mediator.Send(new ResetPasswordCommand(new ResetPasswordRequest(model.Token, model.NewPassword)), cancellationToken).ConfigureAwait(false);

        logger.LogDebug("Completed ResetPassword operation successfully");
        return Ok(response);
    }
}
