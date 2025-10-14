﻿using Aro.Admin.Application.Mediator.Authentication.Commands;
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
public class AuthenticationController(IMediator mediator, IMapper mapper, ILogManager<AuthenticationController> logger) : ControllerBase
{
    [HttpPost("authenticate")]
    [AllowAnonymous]
    public async Task<IActionResult> Authenticate([FromBody] AuthenticationModel model, CancellationToken token)
    {
        logger.LogDebug("Starting Authenticate operation for email: {Email}", model.Email);
        
        var response = await mediator.Send(new AuthenticateUserCommand(new AuthenticateUserRequest { Email = model.Email, Password = model.Password }), token).ConfigureAwait(false);

        logger.LogDebug("Completed Authenticate operation successfully");
        return Ok(new
        {
            response.RefreshTokenExpiry,
            response.RefreshToken,
            response.AccessToken,
            response.AccessTokenExpiry
        });
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenModel model, CancellationToken cancellationToken)
    {
        logger.LogDebug("Starting RefreshToken operation");
        
        var token = await mediator.Send(new RefreshTokenCommand(mapper.Map<RefreshTokenRequest>(model)), cancellationToken).ConfigureAwait(false);

        logger.LogDebug("Completed RefreshToken operation successfully");
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
        logger.LogDebug("Starting Logout operation for userId: {UserId}", model.UserId);
        
        _ = await mediator.Send(new LogoutUserCommand(mapper.Map<LogoutUserRequest>(model)), cancellationToken).ConfigureAwait(false);

        logger.LogDebug("Completed Logout operation successfully");
        return NoContent();
    }

    [HttpPost("logoutall")]
    [Authorize]
    public async Task<IActionResult> LogoutAll([FromBody] LogoutAllUserModel model, CancellationToken cancellationToken)
    {
        logger.LogDebug("Starting LogoutAll operation for userId: {UserId}", model.UserId);
        
        await mediator.Send(new LogoutUserAllCommand(mapper.Map<LogoutUserAllRequest>(model)), cancellationToken).ConfigureAwait(false);

        logger.LogDebug("Completed LogoutAll operation successfully");
        return NoContent();
    }
}
