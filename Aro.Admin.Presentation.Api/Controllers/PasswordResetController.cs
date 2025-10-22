using Aro.Admin.Application.Mediator.PasswordReset.Commands;
using Aro.Admin.Application.Mediator.PasswordReset.DTOs;
using Aro.Admin.Application.Services;
using Aro.Admin.Presentation.Api.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Aro.Admin.Presentation.Api.Controllers;

[ApiController]
[Route("api/password-reset")]
public class PasswordResetController(IMediator mediator, ILogManager<PasswordResetController> logger) : ControllerBase
{
    [HttpPost("generate-token")]
    [AllowAnonymous]
    public async Task<IActionResult> GeneratePasswordResetToken([FromBody] GeneratePasswordResetTokenModel model, CancellationToken cancellationToken)
    {
        logger.LogDebug("Starting GeneratePasswordResetToken operation for email: {Email}", model.Email);

        var response = await mediator.Send(new GeneratePasswordResetTokenCommand(new GeneratePasswordResetTokenRequest(model.Email)), cancellationToken).ConfigureAwait(false);

        logger.LogDebug("Completed GeneratePasswordResetToken operation successfully");
        return Ok(response);
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
