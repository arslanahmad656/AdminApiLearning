using Aro.Admin.Application.Mediator.Authentication.Commands;
using Aro.Admin.Application.Mediator.Authentication.DTOs;
using Aro.Admin.Application.Mediator.Authentication.Notifications;
using Aro.Admin.Application.Services;
using MediatR;

namespace Aro.Admin.Application.Mediator.Authentication.Handlers;

public class LogoutUserCommandHandler(IMediator mediator, IAuthenticationService authenticationService, IHasher hasher, ICurrentUserService currentUserService) : IRequestHandler<LogoutUserCommand, bool>
{
    public async Task<bool> Handle(LogoutUserCommand request, CancellationToken cancellationToken)
    {
        var tokenInfo = currentUserService.GetTokenInfo();
        var response = await authenticationService.Logout(request.Data.UserId, request.Data.RefreshToken, tokenInfo.TokenIdentifier, tokenInfo.Expiry, cancellationToken).ConfigureAwait(false);

        await mediator.Publish(new UserLoggedOutNotification(new UserLoggedOutNotificationData 
        { 
            UserId = request.Data.UserId, 
            RefreshTokenHash = hasher.Hash(request.Data.RefreshToken), 
            TokenIdentifier = tokenInfo.TokenIdentifier 
        }), cancellationToken).ConfigureAwait(false);

        return response;
    }
}
