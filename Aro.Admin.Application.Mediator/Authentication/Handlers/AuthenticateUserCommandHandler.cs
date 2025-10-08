using Aro.Admin.Application.Mediator.Authentication.Commands;
using Aro.Admin.Application.Mediator.Authentication.DTOs;
using Aro.Admin.Application.Mediator.Authentication.Notifications;
using Aro.Admin.Application.Services;
using AutoMapper;
using MediatR;

namespace Aro.Admin.Application.Mediator.Authentication.Handlers;

public class AuthenticateUserCommandHandler(IAuthenticationService authenticationService, IMapper mapper, IMediator mediator) : IRequestHandler<AuthenticateUserCommand, AuthenticateUserResponse>
{
    public async Task<AuthenticateUserResponse> Handle(AuthenticateUserCommand request, CancellationToken cancellationToken)
    {
		try
		{
            var response = await authenticationService.Authenticate(request.Data.Email, request.Data.Password, cancellationToken).ConfigureAwait(false);

            await mediator.Publish(new UserAuthenticatedNotification(mapper.Map<SuccessfulAuthenticationData>(response) with { Email = request.Data.Email }), cancellationToken).ConfigureAwait(false);

            return mapper.Map<AuthenticateUserResponse>(response);
        }
		catch (Exception ex)
		{
            await mediator.Publish(new UserAuthenticationFailedNotification(new(request.Data.Email, ex.Message)), cancellationToken).ConfigureAwait(false);

			throw;
		}
    }
}
