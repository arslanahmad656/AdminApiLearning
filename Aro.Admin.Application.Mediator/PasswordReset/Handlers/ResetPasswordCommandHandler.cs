using Aro.Admin.Application.Mediator.PasswordReset.Commands;
using Aro.Admin.Application.Mediator.PasswordReset.DTOs;
using Aro.Admin.Application.Mediator.PasswordReset.Notifications;
using Aro.Admin.Application.Services;
using Aro.Admin.Application.Services.DataServices;
using Aro.Admin.Domain.Shared.Exceptions;
using MediatR;

namespace Aro.Admin.Application.Mediator.PasswordReset.Handlers;

public class ResetPasswordCommandHandler(
    IPasswordResetTokenService passwordResetTokenService,
    IUserService userService,
    ErrorCodes errorCodes,
    IMediator mediator) : IRequestHandler<ResetPasswordCommand, ResetPasswordResponse>
{
    public async Task<ResetPasswordResponse> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await passwordResetTokenService.ValidateToken(request.Data.Token, cancellationToken).ConfigureAwait(false);
            
            if (!result.IsValid || result.UserId == null)
            {
                var failureNotificationData = new PasswordResetFailedNotificationData(
                    result.UserId,
                    "Invalid or expired password reset token",
                    DateTime.UtcNow
                );
                await mediator.Publish(new PasswordResetFailedNotification(failureNotificationData), cancellationToken).ConfigureAwait(false);
                
                return new ResetPasswordResponse(false, errorCodes.PASSWORD_RESET_TOKEN_ERROR, "Invalid or expired password reset token");
            }
            
            await userService.ResetPassword(result.UserId.Value, request.Data.NewPassword, cancellationToken).ConfigureAwait(false);
            
            await passwordResetTokenService.MarkTokenUsed(request.Data.Token, cancellationToken).ConfigureAwait(false);
            
            var notificationData = new PasswordResetCompletedNotificationData(
                result.UserId.Value,
                DateTime.UtcNow
            );
            await mediator.Publish(new PasswordResetCompletedNotification(notificationData), cancellationToken).ConfigureAwait(false);
            
            return new ResetPasswordResponse(true, null, "Password reset successfully");
        }
        catch (Exception ex)
        {
            var failureNotificationData = new PasswordResetFailedNotificationData(
                null,
                ex.Message,
                DateTime.UtcNow
            );
            await mediator.Publish(new PasswordResetFailedNotification(failureNotificationData), cancellationToken).ConfigureAwait(false);
            
            return new ResetPasswordResponse(false, (ex is AroException aroEx ? aroEx.ErrorCode : errorCodes.PASSWORD_RESET_TOKEN_ERROR), ex.Message);
        }
    }
}
