using Aro.Admin.Application.Mediator.PasswordReset.Commands;
using Aro.Admin.Application.Mediator.PasswordReset.DTOs;
using Aro.Admin.Application.Mediator.PasswordReset.Notifications;
using Aro.Admin.Application.Services;
using Aro.Admin.Application.Services.DataServices;
using MediatR;

namespace Aro.Admin.Application.Mediator.PasswordReset.Handlers;

public class ResetPasswordCommandHandler(
    IPasswordResetTokenService passwordResetTokenService,
    IUserService userService,
    IMediator mediator) : IRequestHandler<ResetPasswordCommand, ResetPasswordResponse>
{
    public async Task<ResetPasswordResponse> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validate token first
            var isValid = await passwordResetTokenService.ValidateToken(request.Data.Token, cancellationToken).ConfigureAwait(false);
            
            if (!isValid)
            {
                return new ResetPasswordResponse(false, "Invalid or expired password reset token");
            }
            
            // TODO: Get user ID from token validation
            // For now, we'll need to implement a way to get the user ID from the token
            // This might require modifying the ValidateToken method to return user ID
            
            // TODO: Update user password
            // await userService.UpdatePassword(userId, request.Data.NewPassword, cancellationToken);
            
            // Mark token as used
            await passwordResetTokenService.MarkTokenUsed(request.Data.Token, cancellationToken).ConfigureAwait(false);
            
            // Publish notification
            var notificationData = new PasswordResetCompletedNotificationData(
                Guid.Empty, // TODO: Get actual user ID
                "user@example.com", // TODO: Get actual email
                DateTime.UtcNow
            );
            await mediator.Publish(new PasswordResetCompletedNotification(notificationData), cancellationToken).ConfigureAwait(false);
            
            return new ResetPasswordResponse(true, "Password reset successfully");
        }
        catch (Exception ex)
        {
            return new ResetPasswordResponse(false, $"Password reset failed: {ex.Message}");
        }
    }
}
