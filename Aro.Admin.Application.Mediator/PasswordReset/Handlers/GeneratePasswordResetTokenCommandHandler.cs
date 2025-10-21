using Aro.Admin.Application.Mediator.PasswordReset.Commands;
using Aro.Admin.Application.Mediator.PasswordReset.DTOs;
using Aro.Admin.Application.Mediator.PasswordReset.Notifications;
using Aro.Admin.Application.Services;
using Aro.Admin.Application.Services.DataServices;
using Aro.Admin.Application.Services.DTOs.ServiceParameters.PasswordReset;
using Aro.Admin.Application.Shared.Options;
using MediatR;
using Microsoft.Extensions.Options;

namespace Aro.Admin.Application.Mediator.PasswordReset.Handlers;

public class GeneratePasswordResetTokenCommandHandler(
    IPasswordResetTokenService passwordResetTokenService,
    IUserService userService,
    IRequestInterpretorService requestInterpretorService,
    IOptionsSnapshot<PasswordResetSettings> passwordResetOptions,
    IMediator mediator) : IRequestHandler<GeneratePasswordResetTokenCommand, GeneratePasswordResetTokenResponse>
{
    public async Task<GeneratePasswordResetTokenResponse> Handle(GeneratePasswordResetTokenCommand request, CancellationToken cancellationToken)
    {
        // Get user by email
        var user = await userService.GetUserByEmail(request.Data.Email, false, true, cancellationToken).ConfigureAwait(false);
        
        // Get IP address and User Agent from request context
        var ipAddress = requestInterpretorService.RetrieveIpAddress() ?? string.Empty;
        var userAgent = requestInterpretorService.GetUserAgent() ?? string.Empty;
        
        // Generate password reset token
        var parameters = new GenerateTokenParameters(
            user.Id,
            ipAddress,
            userAgent
        );
        
        var token = await passwordResetTokenService.GenerateToken(parameters, cancellationToken).ConfigureAwait(false);
        
        // Get expiry from configuration
        var passwordResetSettings = passwordResetOptions.Value;
        var expiry = DateTime.UtcNow.AddMinutes(passwordResetSettings.TokenExpiryMinutes);
        
        // Publish notification
        var notificationData = new PasswordResetTokenGeneratedNotificationData(
            user.Id,
            user.Email,
            token,
            expiry
        );
        await mediator.Publish(new PasswordResetTokenGeneratedNotification(notificationData), cancellationToken).ConfigureAwait(false);
        
        return new GeneratePasswordResetTokenResponse(token, expiry);
    }
}
