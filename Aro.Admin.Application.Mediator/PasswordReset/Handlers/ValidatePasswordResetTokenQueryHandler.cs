using Aro.Admin.Application.Mediator.PasswordReset.DTOs;
using Aro.Admin.Application.Mediator.PasswordReset.Notifications;
using Aro.Admin.Application.Mediator.PasswordReset.Queries;
using Aro.Admin.Application.Services;
using MediatR;

namespace Aro.Admin.Application.Mediator.PasswordReset.Handlers;

public class ValidatePasswordResetTokenQueryHandler(
    IPasswordResetTokenService passwordResetTokenService,
    IMediator mediator) : IRequestHandler<ValidatePasswordResetTokenQuery, ValidatePasswordResetTokenResponse>
{
    public async Task<ValidatePasswordResetTokenResponse> Handle(ValidatePasswordResetTokenQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var isValid = await passwordResetTokenService.ValidateToken(request.Data.Token, cancellationToken).ConfigureAwait(false);
            
            // Publish notification
            var notificationData = new PasswordResetTokenValidatedNotificationData(
                Guid.Empty, // TODO: Get user ID from token validation
                request.Data.Token,
                isValid
            );
            await mediator.Publish(new PasswordResetTokenValidatedNotification(notificationData), cancellationToken).ConfigureAwait(false);
            
            return new ValidatePasswordResetTokenResponse(isValid, isValid ? Guid.Empty : null); // TODO: Get actual user ID
        }
        catch (Exception)
        {
            // Token validation failed
            var notificationData = new PasswordResetTokenValidatedNotificationData(
                Guid.Empty,
                request.Data.Token,
                false
            );
            await mediator.Publish(new PasswordResetTokenValidatedNotification(notificationData), cancellationToken).ConfigureAwait(false);
            
            return new ValidatePasswordResetTokenResponse(false, null);
        }
    }
}
