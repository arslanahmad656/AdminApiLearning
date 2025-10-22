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
            var result = await passwordResetTokenService.ValidateToken(request.Data.Token, cancellationToken).ConfigureAwait(false);
            
            var notificationData = new PasswordResetTokenValidatedNotificationData(
                result.UserId ?? Guid.Empty,
                request.Data.Token,
                result.IsValid
            );
            await mediator.Publish(new PasswordResetTokenValidatedNotification(notificationData), cancellationToken).ConfigureAwait(false);
            
            return new ValidatePasswordResetTokenResponse(result.IsValid, result.UserId);
        }
        catch (Exception)
        {
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
