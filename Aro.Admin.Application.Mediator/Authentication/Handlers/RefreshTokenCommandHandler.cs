using Aro.Admin.Application.Mediator.Authentication.Commands;
using Aro.Admin.Application.Mediator.Authentication.DTOs;
using Aro.Admin.Application.Mediator.Authentication.Notifications;
using Aro.Admin.Application.Services;
using AutoMapper;
using MediatR;

namespace Aro.Admin.Application.Mediator.Authentication.Handlers;

public class RefreshTokenCommandHandler(IMediator mediator, IRefreshTokenService refreshTokenService, IMapper mapper, IHasher hasher) : IRequestHandler<RefreshTokenCommand, RefreshTokenResponse>
{
    public async Task<RefreshTokenResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var response = await refreshTokenService.RefreshToken(request.Data.RefreshToken, cancellationToken).ConfigureAwait(false);

        await mediator.Publish(new TokenRefreshedNotification(new TokenRefreshedNotificationData(
            response.UserId, 
            response.OldRefreshTokenHash, 
            hasher.Hash(response.RefreshToken), 
            response.AccessTokenExpiry, 
            response.RefreshTokenExpiry
        )), cancellationToken).ConfigureAwait(false);

        return mapper.Map<RefreshTokenResponse>(response);
    }
}
